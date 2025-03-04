CREATE PROCEDURE ERPGetDocumentApprove
	@ServiceStatus NVARCHAR(1) = 'F'
AS
BEGIN
	SELECT t0.DocEntry,
		   t0.DocNum, 
		   t0.DocDate, 
		   --t0.DocStatus,
		   --t0.WddStatus, 
		   t0.ObjType
	  FROM ODRF t0
	  LEFT JOIN ErpDocumentApproved T1 ON T0.ObjType = T1.ObjType
								 AND T0.DocNum = T1.DocNum
								 AND T0.DocEntry = T1.DocEntry
	 WHERE WddStatus = 'Y' 
	   AND DocStatus = 'O' 
	   AND (ISNULL(T1.ServiceStatus, '') = '' OR (T1.ServiceStatus = 'F' AND CountF < 10) )
END