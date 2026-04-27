namespace iWallet.Application.DTOs
{
    public record TransferReceiptDto
    {
        public string TransactionReference {  get; set; }
        public string SenderName { get; set; }
        public string SenderWalletNumber { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiveWalletNumber { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal Fees { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } 

    }
}
