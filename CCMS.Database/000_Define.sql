Prompt *****************************************************
Prompt ****************Starting Installation****************
Prompt *****************************************************
prompt ************Enter inputs in CAPS letters*************
prompt **Input Source Database Details only on this screen**
prompt
prompt
accept INSTANCE       char prompt 'Enter TNSNAMES entry to connect to database : '
accept SYSTEM_PASS	  char prompt 'Enter SYSTEM Password : ' hide
accept APP_PASSWORD	  char prompt 'Enter CCMSDB Schema Password: 'hide
accept APP_DF_PATH	  char prompt 'Enter Data File Path : '

DEFINE INSTANCE = &INSTANCE;
DEFINE SYSTEM_PASS = &SYSTEM_PASS;
DEFINE APP_PASSWORD = &APP_PASSWORD;
DEFINE APP_DF_PATH = &APP_DF_PATH;