# SampleApiServer
C#�ł̃Q�[��API�T�[�o�̃T���v�������ł��B


## ���\�z�菇
### �J����
�J�����͈ȉ���z�肵�Ă��܂��B
* [Docker Desktop 2.1](https://hub.docker.com/editions/community/docker-ce-desktop-windows)
* [Visual Studio 2019](https://docs.microsoft.com/ja-jp/visualstudio/ide/?view=vs-2019)
* [Entity Framework Core Tools](https://docs.microsoft.com/ja-jp/ef/core/miscellaneous/cli/dotnet)

### �����
������Ƃ��Ă͈ȉ���z�肵�Ă��܂��B

* [Docker](https://www.docker.com/)
* [ASP.NET Core](https://docs.microsoft.com/ja-jp/aspnet/core/?view=aspnetcore-3.1) 3.1
* [MySQL](https://www.mysql.com/jp/) 5.7
* [Redis](https://redis.io/) 5

### �J�����\�z
Windows PC��ɊJ�������\�z����ꍇ�̎菇�������܂��B

1. PC��[Hyper-V��L����](https://docs.microsoft.com/ja-jp/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v)���ADocker Desktop���C���X�g�[�����܂��B
2. PC��Visual Studio���C���X�g�[�����܂��B
3. �t�@�C���ꎮ��C�ӂ̃t�H���_�ɓW�J���܂��B
4. Visual Studio���N�����āA�X�^�[�g�A�b�v�v���W�F�N�g�� `docker-compose` �ɂ��āA���s���܂��B
    * �}�C�O���[�V�������������s�����̂ŁA�}�C�O���[�V�����R���e�i���I������܂ő҂��Ă��������B  
    ����͐������x������܂��B�r���Œ��f�����DB���s���S�ɂȂ�܂��B���̏ꍇDocker��volume���폜���Ă���Ď��s���Ă��������B�j

�Ȍ�́A`docker-compose` �ŋN�����āAhttp://127.0.0.1:8080/swagger/ ��URL��Swagger��ʂ��A�N�Z�X�\�ł��B

�� Docker Desktop�ŃR���e�i�ւ̃{�����[���̃}�E���g�ŃG���[����������ꍇ�́A���L�h���C�u�ݒ�����Z�b�g���čċ��L���āADocker���ċN�����Ă��������B

## �R�}���h
�A�v���Ŏ��s�\��CLI�R�}���h�Q�B  
dotnet ef�n�̃R�}���h�́A�ʓrEntity Framework Core Tools�̃C���X�g�[�����K�v�ł��B  
�i�C���X�g�[���� `dotnet tool install -g dotnet-ef --version 3.1.0` �ŉB�j  

### �K�v�Ȋ��ϐ�
�� ������ `()` �̒��͐ݒ��
* `ASPNETCORE_ENVIRONMENT` : WebHost��������� (=Development)
* `ASPNETCORE_SUB_ENVIRONMENT` : �����I�Ȋ���� (=dev1)
