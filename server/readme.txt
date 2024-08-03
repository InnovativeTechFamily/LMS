---start 
npm i bcryptjs cookie-parser cors dotenv express ioredis jsonwebtoken mongoose ts-node-dev @types/bcryptjs @types/cookie-parser @types/cors @types/jsonwebtoken @types/node typescript



problem comes when we start or run the first time 
i use in packeage 
C:\Program Files\nodejs\node.exe: bad option: --resawn
PS D:\SQL\GitHub\LMS\server> npm run dev

> server@1.0.0 dev
> ts-node-dev --respawn --transpile-only server.ts

[INFO] 11:46:29 ts-node-dev ver. 2.0.0 (using ts-node ver. 10.9.2, typescript ver. 5.5.4)
Compilation error in D:\SQL\GitHub\LMS\server\server.ts
[ERROR] 11:46:29 ⨯ Unable to compile TypeScript:
error TS5109: Option 'moduleResolution' must be set to 'NodeNext' (or left unspecified) when option 'module' is set to 'NodeNext'.


Solution 
tsnd --respawn server.ts
PS D:\SQL\GitHub\LMS\server> npm run dev

> server@1.0.0 dev
> tsnd --respawn server.ts

[INFO] 11:49:57 ts-node-dev ver. 2.0.0 (using ts-node ver. 10.9.2, typescript ver. 5.5.4)
Server is connected with port 8000


db
MONGODB_URL = mongodb+srv://manishguptagm01:wRcXZyGN1WO2dgkW@cluster0.gnlbmej.mongodb.net/LMS


npx tsc --init
npm i ejs
npm i nodemailer

npm i --save-dev @types/ejs
npm i --save-dev @types/nodemailer


done time 2:46:06
Login ,Logout User

done 3:07:00