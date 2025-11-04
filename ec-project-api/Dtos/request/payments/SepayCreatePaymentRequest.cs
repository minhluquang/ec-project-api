using Newtonsoft.Json;

namespace ec_project_api.Dtos.request.payments
{
    public class SepayCreatePaymentRequest
    {   // Model này không cần thay đổi
        public class CreatePaymentRequest
        {
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
        }
        public class CreateQRRequest
        {
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string BankCode { get; set; }
            public string BankAccountNumber { get; set; }
        }

        // Model này không cần thay đổi
        public class PaymentResponse
        {
            public string AccountName { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public string QrCodeUrl { get; set; }
            public string TransactionId { get; set; }
            public string ImageUrl { get; set; }
        }

        // Model này giữ nguyên để nhận đủ dữ liệu từ Sepay
        public class SepayWebhookModel
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("gateway")]
            public string Gateway { get; set; }

            [JsonProperty("transactionDate")]
            public DateTime TransactionDate { get; set; }

            [JsonProperty("accountNumber")]
            public string AccountNumber { get; set; }

            [JsonProperty("code")]
            public string? Code { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("transferType")]
            public string TransferType { get; set; } // "in" hoặc "out"

            [JsonProperty("transferAmount")]
            public decimal TransferAmount { get; set; }

            [JsonProperty("accumulated")]
            public decimal Accumulated { get; set; }

            [JsonProperty("subAccount")]
            public string? SubAccount { get; set; }

            [JsonProperty("referenceCode")]
            public string ReferenceCode { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

        // Model này không cần thay đổi
        public class CheckPaymentStatusResponse
        {
            public string Status { get; set; }
            public bool IsPaid { get; set; }
            public DateTime? PaidAt { get; set; }
        }
    }
}
