set minioalias=minio147

@echo off  
if "%1%"=="" ( goto exit)
if not exist ./mc.exe  goto nomc
echo 将创建存储桶 %1% 
echo mc mb %minioalias%/%1%
mc mb %minioalias%/%1%
echo 将设置存储桶的匿名权限
echo mc policy set download %minioalias%/%1%
mc policy set download %minioalias%/%1%
echo 将新增管理员 %1_admin 密码为： %1_123.abc
echo mc admin user add %minioalias%/ %1_admin %1_123.abc
mc admin user add %minioalias%/ %1_admin %1_123.abc
echo 将设置新的管理策略 %1%_admin_policy
set str={"Version":"2012-10-17","Statement":[{"Effect":"Allow","Action":["s3:GetBucketLocation","s3:ListBucket"],"Resource":["arn:aws:s3:::%1"]},{"Effect":"Allow","Action":["s3:GetObject","s3:PutObject","s3:DeleteObject"],"Resource":["arn:aws:s3:::%1%/*"]}]}
echo %str%
echo %str% > policy.json
echo mc admin policy add %minioalias%/ %1%_admin_policy ./policy.json
mc admin policy add %minioalias%/ %1%_admin_policy ./policy.json
echo 将对管理员 %1%_admin 应用新的策略 %1%_admin_policy
echo mc admin policy set %minioalias%/ %1_admin_policy user=%1%_admin
mc admin policy set %minioalias%/ %1_admin_policy user=%1%_admin
echo 完成
goto exit
:nomc
echo 未找到 mc.exe 文件
:exit
