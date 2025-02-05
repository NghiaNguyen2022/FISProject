CREATE TABLE "tb_Payoo_BatchHeader"
(
    "BatchNo" NVARCHAR(100),
    "TotalAmount" DECIMAL(19,2),
    "RowCount" INT,
    "PageSize" INT,
)
CREATE TABLE "tb_Payoo_BatchDetail"
(
    "BatchNo" NVARCHAR(100),
    "OrderNo" NVARCHAR(100),
    "ShopId" NVARCHAR(100),
    "SellerName" NVARCHAR(100),
    "TransferAmount" DECIMAL(19,6),
    "InvoiceDate" NVARCHAR(40),
    "Status"  NVARCHAR(10),
    "IntDate"  NVARCHAR(10),
    "IntTime"  NVARCHAR(10),
    "BankRecStatus"  NVARCHAR(10),
    "BankRefNo"  NVARCHAR(100),
    "PaymentID"  NVARCHAR(10),
    "Message"  NVARCHAR(100)
)

CREATE PROCEDURE "sp_Payoo_BatchExist"
(
	IN v_BatchNo  NVARCHAR(100)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
    --DECLARE _Existed NVARCHAR(100);
    IF EXISTS (SELECT 1 
                 FROM "tb_Payoo_BatchHeader"
                WHERE "BatchNo" = :v_BatchNo)
    THEN
        SELECT 'Existed' AS "Existed" FROM DUMMY;
    ELSE
        SELECT 'No' AS "Existed" FROM DUMMY;
    END IF;
END

ALTER PROCEDURE "sp_Bank_PayooClear"
(
	IN v_transno NVARCHAR(40)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	SELECT t1."BatchNo", T0."credit", SUM(T1."TransferAmount") AS "Amount", T0."credit" - SUM(T1."TransferAmount") AS "Diff"
	  FROM "tb_Bank_InquiryDetail" T0
	  JOIN "tb_Payoo_BatchDetail" T1 ON T0."description" LIKE '%' || t1."BatchNo" || '%'
	 WHERE T0."credit" > 0 
	   AND T1."BankRecStatus" = 'N'
	   AND T0."transactionNumber" = :v_transno
	 GROUP BY t1."BatchNo", T0."credit";
END