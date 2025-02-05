ALTER PROCEDURE "usp_GeneratePaymentKey"
(
	IN v_BrId NVARCHAR(10),
	IN v_Type NVARCHAR(2),
	IN v_Date DATE
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	DECLARE _Month INT = MONTH(v_Date);
	DECLARE _Year INT = YEAR(v_Date);
	DECLARE _Branch NVARCHAR(2);
	DECLARE _Number INT;
	DECLARE _Count INT;
	
	SELECT COALESCE(SUBSTRING("AliasName", 2,2), '')
	  INTO _Branch
      FROM OBPL
     WHERE "BPLId" = :v_BrId; 
       
	SELECT COUNT(*) "Count"
	  INTO _Count
	  FROM "PaymentKey"
     WHERE "BranchId" = :_Branch
       AND "TypeId" = :v_Type
       AND "Year" = :_Year
       AND "Month" = :_Month;
       
	IF(COALESCE(:_Count, 0)) = 0
	THEN
		INSERT INTO "PaymentKey"
		VALUES (:_Branch, :v_Type, :_Month, :_Year, 1);
		
		SELECT 1 
		  INTO _Number
		  FROM DUMMY; 
	ELSE
		SELECT "Number" + 1
		  INTO _Number
		  FROM "PaymentKey"
		 WHERE "BranchId" = :_Branch
		   AND "TypeId" = :v_Type
		   AND "Year" = :_Year
		   AND "Month" = :_Month;
		   
		UPDATE T0
		   SET T0."Number" = T0."Number" + 1
		  FROM "PaymentKey" T0
	     WHERE "BranchId" = :_Branch
	       AND "TypeId" = :v_Type
	       AND "Year" = :_Year
	       AND "Month" = :_Month;
	END IF;
	
	SELECT :_Number AS "Number",
		   :_Branch AS "Branch"
      FROM DUMMY; 
END;

