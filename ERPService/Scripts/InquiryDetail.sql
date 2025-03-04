CREATE TABLE "tb_Bank_InquiryDetail"
( 
    "requestId" NVARCHAR(30),
    "providerId" NVARCHAR(30),
    "merchantId" NVARCHAR(30),
    "transDate" NVARCHAR(30),
    "description" NVARCHAR(200),
    "debit" DECIMAL(19,2),
    "credit" DECIMAL(19,2),
    "accountBal" DECIMAL(19,2),
    "transNumber" NVARCHAR(30), 
    "senderAccount" NVARCHAR(30),
    "senderName" NVARCHAR(200),
    "agency" NVARCHAR(30),
    "virAccount" NVARCHAR(30),
    "senderBankName" NVARCHAR(200),
    "senderBankId" NVARCHAR(30),
    "channel" NVARCHAR(30),
 )