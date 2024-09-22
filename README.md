**ExecuteMultipleSP**

Can able to perform Insert, Read, Update and Delete SP by providing SPName and and corresponding paramers as key value pair. 
Read SP not needed anyparameter. so it can pass as empty/null.not should be present in the request.

_Sample Request_

{
    "procedureNames": [
        {
            "spName": "sp_CreateUser",
            "parameters": [
                {
                    "UserName": "testuser",
                    "Email": "test@mail.com"
                }
            ]
        }
    ]
}


**Limitation**

Not able to insert/update/delete multiple paramerts at a time.
