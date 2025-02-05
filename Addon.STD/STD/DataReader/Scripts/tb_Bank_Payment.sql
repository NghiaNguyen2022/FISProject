drop TABLE "tb_Bank_Payment";
CREATE TABLE "tb_Bank_Payment"
(
	"CardCode" VARCHAR(50),
	"DocEntry" INT,
	"TransId" VARCHAR(30),
	"Status"  VARCHAR(10),
	"Message"  VARCHAR(200),
	"DateAction" Date,
	"Key" VARCHAR(30)
)

-- Call "SP_LogPaymentTool" ('NC0100051', '1283', 'C', 'F', 'Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Dòng tiền.') 

ALTER PROCEDURE "usp_Bank_LogPaymentTool"
(
	IN v_CardCode NVARCHAR(100),
	IN v_DocEntry INT,
	IN v_TransId VARCHAR(30) ,
	IN v_Status VARCHAR(1),
	IN v_StatusName VARCHAR(500),
	IN v_key VARCHAR(30)
	
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF EXISTS (SELECT 1	
				 FROM "tb_Bank_Payment"
				WHERE "CardCode" = :v_CardCode
				  AND "DocEntry" = :v_DocEntry
				  AND "TransId" = :v_TransId
				)
	THEN
		UPDATE "tb_Bank_Payment"
 		   SET "Status" = :v_Status,
 		   	   "Message" = :v_StatusName,
 		   	   "DateAction" = CURRENT_DATE,
 		   	   "Key" = :v_key
 		 WHERE "CardCode" = :v_CardCode
		   AND "DocEntry" = :v_DocEntry
		   AND "TransId" = :v_TransId;
	ELSE
		INSERT INTO "tb_Bank_Payment"
		VALUES (:v_CardCode, :v_DocEntry, :v_TransId, :v_Status, :v_StatusName, CURRENT_DATE, :v_key);
	END IF;
END;