set minioalias=minio147

@echo off  
if "%1%"=="" ( goto exit)
if not exist ./mc.exe  goto nomc

echo ��ɾ������Ա %1_admin
echo mc admin user remove %minioalias%/ %1_admin
mc admin user remove %minioalias%/ %1_admin

echo ��ɾ���������
echo mc admin policy remove  %minioalias%/ %1%_admin_policy
mc admin policy remove  %minioalias%/ %1%_admin_policy

echo ��ɾ���洢Ͱ
echo mc rm --recursive --force %minioalias%/%1%
mc rm --recursive --force %minioalias%/%1%
echo ���
goto exit

:nomc
echo δ�ҵ� mc.exe �ļ�
:exit
