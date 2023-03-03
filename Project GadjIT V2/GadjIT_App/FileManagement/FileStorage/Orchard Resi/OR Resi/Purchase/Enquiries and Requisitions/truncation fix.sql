
UPDATE Fd
SET fd.Size = 500 
from Mp_Sys_FieldDef Fd
INNER JOIN Mp_Sys_TableDef Td on Fd.TableRef = Td.ID
WHERE InternalName = 'Usr_OR_Import_Invoice_Columns' 
AND InternalFieldName = 'Description'

ALTER TABLE Usr_OR_Import_Invoice_Columns 
ALTER COLUMN Description VARCHAR (500)
