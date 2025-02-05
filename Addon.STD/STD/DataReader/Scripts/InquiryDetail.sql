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
    "transactionNumber" NVARCHAR(30), 
    "senderAccount" NVARCHAR(30),
    "senderName" NVARCHAR(200),
    "agency" NVARCHAR(30),
    "virAccount" NVARCHAR(30),
    "senderBankName" NVARCHAR(200),
    "senderBankId" NVARCHAR(30),
    "channel" NVARCHAR(30),
 )

 CREATE PROCEDURE "sp_Bank_InquiryExist"
(
	IN v_TransNo  NVARCHAR(100)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
    IF EXISTS (SELECT 1 
                 FROM "tb_Bank_InquiryDetail"
                WHERE "transactionNumber" = :v_TransNo)
    THEN
        SELECT 'Existed' AS "Existed" FROM DUMMY;
    ELSE
        SELECT 'No' AS "Existed" FROM DUMMY;
    END IF;
END