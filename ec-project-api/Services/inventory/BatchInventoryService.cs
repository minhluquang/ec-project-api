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
            while (remainingQuantity > 0)
            {
                var activeBatches = await _context.PurchaseOrderItems
                    .Where(poi => poi.ProductVariantId == productVariantId 
                                  && poi.IsPushed 
                                  && poi.Quantity > 0)
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
                firstBatch.Quantity -= (short)quantityFromThisBatch;
                firstBatch.UpdatedAt = DateTime.UtcNow;
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
                    $"Đã trừ {quantityFromThisBatch} từ lô {firstBatch.PurchaseOrderItemId}, " +
                    $"còn lại {firstBatch.Quantity} trong lô, cần trừ thêm {remainingQuantity}");
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Hoàn thành trừ hàng cho variant {productVariantId}, sử dụng {results.Count} lô");

            return results;
        }
        public async Task ReturnToBatchesAsync(int productVariantId, int quantityToReturn)
        {
            if (quantityToReturn <= 0)
                throw new ArgumentException("Số lượng cần hoàn trả phải lớn hơn 0", nameof(quantityToReturn));

            _logger.LogInformation($"Bắt đầu hoàn trả {quantityToReturn} sản phẩm variant {productVariantId}");

            var remainingQuantity = quantityToReturn;
            var activeBatches = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                .OrderByDescending(poi => poi.CreatedAt)
                .ToListAsync();

            if (!activeBatches.Any())
            {
                throw new InvalidOperationException(
                    $"Không tìm thấy lô nào đang active cho variant {productVariantId} để hoàn trả");
            }
            foreach (var batch in activeBatches)
            {
                if (remainingQuantity <= 0) break;
                var quantityToThisBatch = remainingQuantity;
                
                batch.Quantity += (short)quantityToThisBatch;
                batch.UpdatedAt = DateTime.UtcNow;

                remainingQuantity -= quantityToThisBatch;

                _logger.LogInformation(
                    $"Đã hoàn trả {quantityToThisBatch} vào lô {batch.PurchaseOrderItemId}, " +
                    $"tổng trong lô: {batch.Quantity}");

                if (remainingQuantity <= 0) break;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Hoàn thành hoàn trả {quantityToReturn} sản phẩm variant {productVariantId}");
        }
        public async Task<int> GetAvailableStockAsync(int productVariantId)
        {
            var totalAvailable = await _context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                .SumAsync(poi => (int)poi.Quantity);

            return totalAvailable;
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

            // Cập nhật giá bán của ProductVariant (nếu cần)
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);

            if (variant?.Product != null)
            {
                // Tính giá bán = giá nhập * (1 + lợi nhuận)
                var newSellingPrice = nextBatch.UnitPrice * (1 + nextBatch.ProfitPercentage / 100);
                
                // Cập nhật BasePrice của Product
                variant.Product.BasePrice = newSellingPrice;
                variant.Product.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation(
                    $"Kích hoạt lô {nextBatch.PurchaseOrderItemId} cho variant {productVariantId}, " +
                    $"số lượng: {nextBatch.Quantity}, giá nhập: {nextBatch.UnitPrice}, " +
                    $"lợi nhuận: {nextBatch.ProfitPercentage}%, giá bán mới: {newSellingPrice}");
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
    }
}
