set minioalias=minio147

@echo off  
if "%1%"=="" ( goto exit)
if not exist ./mc.exe  goto nomc

echo 将删除管理员 %1_admin
echo mc admin user remove %minioalias%/ %1_admin
mc admin user remove %minioalias%/ %1_admin

echo 将删除管理策略
echo mc admin policy remove  %minioalias%/ %1%_admin_policy
mc admin policy remove  %minioalias%/ %1%_admin_policy

echo 将删除存储桶
echo mc rm --recursive --force %minioalias%/%1%
mc rm --recursive --force %minioalias%/%1%
echo 完成
goto exit

:nomc
echo 未找到 mc.exe 文件
:exit
