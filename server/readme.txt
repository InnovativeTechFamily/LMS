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


issue is 
[INFO] 11:06:13 Restarting: D:\SQL\GitHub\LMS\server\tsconfig.json has been modified
Redis connected
Compilation error in D:\SQL\GitHub\LMS\server\middleware\auth.ts
[ERROR] 11:06:21 ? Unable to compile TypeScript:
middleware/auth.ts(28,9): error TS2339: 
Property 'user' does not exist on type 'Request<ParamsDictionary, any, any, ParsedQs, Record<string, any>>'.

fix iwant to extend Request into new model
interface CustomRequest extends Request {
  user?: any;
}


--
Generate New Token,Get User,Social Authentication

done 3:49:16

update User Info password and avatar

4:19:57
Degine Courde Model
4:38:35 Create And Edit Course
4:57:14 Get Single & All Courses

now here we add both data as a cookies save in redis because redis is server less

5:14:50 Get Course Content