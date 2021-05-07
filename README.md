# CargaArduino2DataBase
Do you want to send directly your data to an SQL data server, please use this project.
What do you need send a Json with your data as the example:
Arduino Command parameters Json:
  IP (string)     IpOfYourDataBase
  DB (string)     NameOfYourDataBase
  User (string)   UserOfYourDataBase
  Pass (string)   ThePassWordOfYourDataBase
  CMD (string)    Insert
  Table (string)  TableToPoint
  Data (string)   JsonDataForEveryColumn
  
Example:
{
  "IP": "192.168.0.100",
  "DB": "TestDataBase",
  "User": "User1",
  "Pass": 1234,
  "CMD": "Insert",
  "Table": "Load",
  "Data": "{\"Temp1\":\"123.3\"}"
}

You can test this implementation more faster with Docker. I let the link here:
https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=video&cd=&cad=rja&uact=8&ved=2ahUKEwj_v5aw6rfwAhVFMqwKHV0dDtUQtwIwAHoECAEQAw&url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DaRtHWO7Xufo&usg=AOvVaw1WXmagIKyGDeb7aV4n72Eh
In arduino see that the data inside to insert in the DataBase has \" as ", this is based on the normativity on JSON.

This project was created on October 2019, and i decided to oppened, to share the knowledge and help to the studends to record their
experiments on a database, your commercial uses please contact me in impresionescaptis@gmail.com.
