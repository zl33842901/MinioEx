set minioalias=minio147

@echo off  
if "%1%"=="" ( goto exit)
if not exist ./mc.exe  goto nomc
echo �������洢Ͱ %1% 
echo mc mb %minioalias%/%1%
mc mb %minioalias%/%1%
echo �����ô洢Ͱ������Ȩ��
echo mc policy set download %minioalias%/%1%
mc policy set download %minioalias%/%1%
echo ����������Ա %1_admin ����Ϊ�� %1_123.abc
echo mc admin user add %minioalias%/ %1_admin %1_123.abc
mc admin user add %minioalias%/ %1_admin %1_123.abc
echo �������µĹ������ %1%_admin_policy
set str={"Version":"2012-10-17","Statement":[{"Effect":"Allow","Action":["s3:GetBucketLocation","s3:ListBucket"],"Resource":["arn:aws:s3:::%1"]},{"Effect":"Allow","Action":["s3:GetObject","s3:PutObject","s3:DeleteObject"],"Resource":["arn:aws:s3:::%1%/*"]}]}
echo %str%
echo %str% > policy.json
echo mc admin policy add %minioalias%/ %1%_admin_policy ./policy.json
mc admin policy add %minioalias%/ %1%_admin_policy ./policy.json
echo ���Թ���Ա %1%_admin Ӧ���µĲ��� %1%_admin_policy
echo mc admin policy set %minioalias%/ %1_admin_policy user=%1%_admin
mc admin policy set %minioalias%/ %1_admin_policy user=%1%_admin
echo ���
goto exit
:nomc
echo δ�ҵ� mc.exe �ļ�
:exit
