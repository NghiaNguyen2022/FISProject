CREATE TABLE "tb_Bank_BIDVInquiryTran"
( 
    "requestId" NVARCHAR(30),
    "seq"  NVARCHAR(30),
    "tranDate" NVARCHAR(30),
    "remark" NVARCHAR(200),
    "debitAmount" NVARCHAR(30),
    "creditAmount"  NVARCHAR(30),
    "RefDoc" NVARCHAR(30), 
    "currCode" NVARCHAR(30)
 )

 CREATE PROCEDURE "sp_Bankbidv_InquiryExist"
(
	IN v_seq  NVARCHAR(30)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
    IF EXISTS (SELECT 1 
                 FROM "tb_Bank_BIDVInquiryTran"
                WHERE "seq" = :v_seq)
    THEN
        SELECT 'Existed' AS "Existed" FROM DUMMY;
    ELSE
        SELECT 'No' AS "Existed" FROM DUMMY;
    END IF;
END