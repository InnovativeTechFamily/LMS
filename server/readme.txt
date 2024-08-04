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
5:25:21 Create Question Add Answer
npm i node-cron
npm i --save-dev @types/node-cron

5:51:08 add review & Reply IN Review 
05:51:07 - 06:14:48 add review in course

06:14:48 - 06:22:08 Notification and Order model 
06:22:08 - 06:47:38 Create Order
06:47:38 - 07:07:40 Get all notifications, update notification status
07:07:40 - 07:14:23 Delete read notifications -- with crone after a certain time
07:14:23 - 07:23:38 Get all users, courses, orders
07:23:38 - 07:28:45  Add and get members in Admin dashboard 
07:28:45 - 07:38:16  Delete course, Delete user
07:38:16 - 08:00:16 Get the last 28 days' users,orders, notifications for the last 1year 
08:00:16 - 08:07:23 Layout model Design
08:07:23 - 08:25:00 Create Faq,Hero banner,Categories
08:25:00 - 08:39:20 Edit Faq,Hero banner,Categories
08:39:20 - 08:55:00 Advance cache maintenance