using ec_project_api.Constants.Messages;
using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ec_project_api.Services.inventory
{
    public class BatchInventoryService : IBatchInventoryService
    {
        private readonly DataContext _context;
        private readonly ILogger<BatchInventoryService> _logger;

        public BatchInventoryService(DataContext context, ILogger<BatchInventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<BatchDeductionResult>> DeductFromBatchesAsync(int productVariantId, int quantityToDeduct)
        {
            if (quantityToDeduct <= 0)
                throw new ArgumentException("Số lượng cần trừ phải lớn hơn 0", nameof(quantityToDeduct));

            var results = new List<BatchDeductionResult>();
            var remainingQuantity = quantityToDeduct;
            _logger.LogInformation($"Bắt đầu trừ {quantityToDeduct} sản phẩm variant {productVariantId} theo FIFO");
            
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId)
                ?? throw new InvalidOperationException($"Không tìm thấy ProductVariant {productVariantId}");

            if (variant.StockQuantity < quantityToDeduct)
            {
                throw new InvalidOperationException(
                    $"Không đủ hàng trong kho cho sản phẩm variant {productVariantId}. " +
                    $"Hiện có: {variant.StockQuantity}, cần: {quantityToDeduct}");
            }

            while (remainingQuantity > 0)
            {
                var activeBatches = await _context.PurchaseOrderItems
                    .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                    .OrderBy(poi => poi.CreatedAt)
                    .ToListAsync();

                if (!activeBatches.Any())
                {
                    _logger.LogWarning($"Hết lô active cho variant {productVariantId}, đang kích hoạt lô tiếp theo...");
                    
                    var activated = await ActivateNextBatchAsync(productVariantId, remainingQuantity);
                    
                    if (!activated)
                    {
                        throw new InvalidOperationException(
                            $"Không đủ hàng trong kho cho sản phẩm variant {productVariantId}. " +
                            $"Cần thêm {remainingQuantity} sản phẩm.");
                    }
                    continue;
                }
                
                var firstBatch = activeBatches.First();
                
                var quantityFromThisBatch = Math.Min(remainingQuantity, firstBatch.Quantity);
                
                var sellingPrice = firstBatch.UnitPrice * (1 + firstBatch.ProfitPercentage / 100);
                results.Add(new BatchDeductionResult
                {
                    PurchaseOrderItemId = firstBatch.PurchaseOrderItemId,
                    QuantityDeducted = quantityFromThisBatch,
                    UnitPrice = firstBatch.UnitPrice,
                    SellingPrice = sellingPrice,
                    ProfitPercentage = firstBatch.ProfitPercentage
                });

                remainingQuantity -= quantityFromThisBatch;

                _logger.LogInformation(
                    $"Đã lấy giá {quantityFromThisBatch} từ lô {firstBatch.PurchaseOrderItemId}, " +
                    $"cần trừ thêm {remainingQuantity}");

                if (remainingQuantity == 0)
                {
                    var totalActiveStock = activeBatches.Sum(b => (int)b.Quantity);
                    
                    // Nếu stock hiện tại (sau khi trừ) bằng 0 → cần push lô tiếp
                    if (variant.StockQuantity - quantityToDeduct == 0)
                    {
                        _logger.LogInformation($"Stock sắp hết cho variant {productVariantId}, đang kích hoạt lô tiếp theo...");
                        var activated = await ActivateNextBatchAsync(productVariantId, 0);
                        if (activated)
                        {
                            _logger.LogInformation($"Đã kích hoạt lô tiếp theo cho variant {productVariantId}");
                        }
                    }
                }
            }
            variant.StockQuantity -= quantityToDeduct;
            variant.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Hoàn thành trừ hàng cho variant {productVariantId}, sử dụng {results.Count} lô. Stock còn lại: {variant.StockQuantity}");

            return results;
        }
        public async Task ReturnToBatchesAsync(int productVariantId, int quantityToReturn)
        {
            if (quantityToReturn <= 0)
                throw new ArgumentException("Số lượng cần hoàn trả phải lớn hơn 0", nameof(quantityToReturn));

            _logger.LogInformation($"Bắt đầu hoàn trả {quantityToReturn} sản phẩm variant {productVariantId}");

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId)
                ?? throw new InvalidOperationException($"Không tìm thấy ProductVariant {productVariantId}");

            var activeBatches = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                .OrderByDescending(poi => poi.CreatedAt)
                .ToListAsync();

            if (!activeBatches.Any())
            {
                _logger.LogWarning($"Không tìm thấy lô nào đang active cho variant {productVariantId}, vẫn hoàn trả vào stock");
            }
            
            variant.StockQuantity += quantityToReturn;
            variant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Hoàn thành hoàn trả {quantityToReturn} sản phẩm variant {productVariantId}. Stock hiện tại: {variant.StockQuantity}");
        }
        public async Task<int> GetAvailableStockAsync(int productVariantId)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

            return variant?.StockQuantity ?? 0;
        }
        public async Task<bool> ActivateNextBatchAsync(int productVariantId, int quantityNeeded)
        {
            _logger.LogInformation($"Đang tìm lô tiếp theo cho variant {productVariantId}, cần {quantityNeeded} sản phẩm");

            // Lấy lô tiếp theo chưa được kích hoạt (is_pushed = false) theo thứ tự created_at ASC
            var nextBatch = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId 
                              && !poi.IsPushed 
                              && poi.Quantity > 0)
                .OrderBy(poi => poi.CreatedAt)
                .FirstOrDefaultAsync();

            if (nextBatch == null)
            {
                _logger.LogWarning($"Không tìm thấy lô nào chưa kích hoạt cho variant {productVariantId}");
                return false;
            }

            // Kích hoạt lô
            nextBatch.IsPushed = true;
            nextBatch.UpdatedAt = DateTime.UtcNow;

            // Lấy thông tin variant và product
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

            if (variant?.Product != null)
            {
                // newBasePrice = (giáHiệnTại × tổngTồnKho + giáNhập × sốLượngNhập) / (tổngTồnKho + sốLượngNhập)
                
                // Lấy tất cả variants của product để tính tổng tồn kho
                var allVariants = await _context.ProductVariants
                    .Where(v => v.ProductId == variant.ProductId)
                    .ToListAsync();
                
                decimal currentSellingPrice = variant.Product.BasePrice;
                int totalCurrentStock = allVariants.Sum(v => v.StockQuantity);
                
                decimal importPrice = nextBatch.UnitPrice;
                int importQuantity = nextBatch.Quantity;
                
                // Tính giá bán mới nhập = giá nhập × (1 + % lợi nhuận)
                decimal newImportSellingPrice = importPrice * (1 + nextBatch.ProfitPercentage / 100);
                
                decimal newBasePrice;
                
                // Nếu có tồn kho hiện tại, tính weighted average với GIÁ BÁN
                if (totalCurrentStock > 0)
                {
                    newBasePrice = (currentSellingPrice * totalCurrentStock + newImportSellingPrice * importQuantity) 
                                 / (totalCurrentStock + importQuantity);
                    
                    _logger.LogInformation(
                        $"Tính giá weighted average: ({currentSellingPrice} × {totalCurrentStock} + {newImportSellingPrice} × {importQuantity}) / {totalCurrentStock + importQuantity} = {newBasePrice}");
                }
                else
                {
                    // Nếu hết hàng hoàn toàn, dùng giá bán mới = giá nhập × (1 + lợi nhuận)
                    newBasePrice = newImportSellingPrice;                    
                    _logger.LogInformation(
                         $"Hết hàng hoàn toàn, dùng giá bán mới: {importPrice} × (1 + {nextBatch.ProfitPercentage}%) = {newBasePrice}");
                }
                
                variant.Product.BasePrice = newBasePrice;
                variant.Product.UpdatedAt = DateTime.UtcNow;
                
                // Cộng số lượng từ lô mới vào variant
                variant.StockQuantity += nextBatch.Quantity;
                variant.UpdatedAt = DateTime.UtcNow;
                
                _logger.LogInformation(
                    $"Kích hoạt lô {nextBatch.PurchaseOrderItemId} cho variant {productVariantId}, " +
                    $"số lượng lô: {nextBatch.Quantity}, giá nhập: {nextBatch.UnitPrice}, " +
                    $"lợi nhuận: {nextBatch.ProfitPercentage}%, giá bán mới Product: {newBasePrice}, " +
                    $"stock mới: {variant.StockQuantity}");
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetCurrentSellingPriceAsync(int productVariantId)
        {
            var firstActiveBatch = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId 
                              && poi.IsPushed 
                              && poi.Quantity > 0)
                .OrderBy(poi => poi.CreatedAt)
                .FirstOrDefaultAsync();

            if (firstActiveBatch == null)
            {
                // Nếu không có lô nào active, thử lấy từ ProductVariant.Product.BasePrice
                var variant = await _context.ProductVariants
                    .Include(pv => pv.Product)
                    .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

                if (variant?.Product?.BasePrice != null)
                {
                    return variant.Product.BasePrice;
                }

                throw new InvalidOperationException(
                    $"Không tìm thấy giá bán cho sản phẩm variant {productVariantId}");
            }

            // Tính giá bán = giá nhập * (1 + lợi nhuận)
            return firstActiveBatch.UnitPrice * (1 + firstActiveBatch.ProfitPercentage / 100);
        }

        public async Task<decimal> CalculateAveragePriceAsync(int productVariantId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0", nameof(quantity));

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

            if (variant == null || variant.StockQuantity < quantity)
            {
                throw new InvalidOperationException(
                    $"Không đủ hàng trong kho. Hiện có: {variant?.StockQuantity ?? 0}, cần: {quantity}");
            }

            var remainingQuantity = quantity;
            decimal totalPrice = 0m;

            // Lấy các lô active theo FIFO (chỉ cần IsPushed = true, không cần kiểm tra Quantity > 0)
            var activeBatches = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                .OrderBy(poi => poi.CreatedAt)
                .ToListAsync();

            foreach (var batch in activeBatches)
            {
                if (remainingQuantity <= 0) break;

                var quantityFromThisBatch = Math.Min(remainingQuantity, batch.Quantity);
                var sellingPrice = batch.UnitPrice * (1 + batch.ProfitPercentage / 100);
                
                totalPrice += sellingPrice * quantityFromThisBatch;
                remainingQuantity -= quantityFromThisBatch;
            }

            if (remainingQuantity > 0)
            {
                throw new InvalidOperationException(
                    $"Không đủ hàng trong các lô active. Thiếu {remainingQuantity} sản phẩm.");
            }

            return totalPrice / quantity;
        }
    }
}

