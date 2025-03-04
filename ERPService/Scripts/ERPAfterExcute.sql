ALTER PROCEDURE ERPAfterExcute
	@ObjType NVARCHAR(50), 
	@DocEntry INT, 
	@Docnum INT,
	@Status CHAR(1),
	@Message NVARCHAR(400)
AS
BEGIN
	IF EXISTS (SELECT 1 
				 FROM ErpDocumentApproved 
				WHERE DocEntry = @DocEntry
				  AND DocNum = @Docnum
				  AND ObjType = @ObjType)
	BEGIN
		UPDATE ErpDocumentApproved 
		   SET ServiceStatus = @Status,
			   Message = @Message,
			   ServiceDate = GETDATE(),
			   CountF = ISNULL(CountF, 0) + 1
		 WHERE DocEntry = @DocEntry
		   AND DocNum = @Docnum
		   AND ObjType = @ObjType
	END
	ELSE
	BEGIN
		INSERT INTO [ErpDocumentApproved]
           ([ObjType]
           ,[DocNum]
           ,[DocEntry]
           ,[ServiceStatus]
           ,[Message]
           ,[ServiceDate]
		   ,[CountF])
     VALUES
           (@ObjType
           ,@Docnum
           ,@DocEntry
           ,@Status
           ,@Message
           ,GETDATE()
		   , 0)
	END
END