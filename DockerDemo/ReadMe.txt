1：准备一台linux虚拟机（这里采用的系统是 centos7.7）
2：安装docker 参考 docker文档里的安装教程
	https://docs.docker.com/engine/install/centos/
3：现在本机创建项目webapi ， DockerDemo 注意文件夹的名字也用DockerDemo(.net core版本是3.1)
4：本机运行 访问 http://localhost:5000/weatherforecast
5：然后上传(SecureCRTPortable)项目文件夹到 linux虚拟机上 ，这里是 /demoProjects/DockerDemo
6：进入 DockerDemo文件夹，创建 Dockerfile文件，文件内容如下 
    参考：https://docs.docker.com/engine/examples/dotnetcore/

	FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
	WORKDIR /app

	# Copy csproj and restore as distinct layers
	COPY *.csproj ./
	RUN dotnet restore

	# Copy everything else and build
	COPY . ./
	RUN dotnet publish -c Release -o out

	# Build runtime image
	FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
	WORKDIR /app
	COPY --from=build-env /app/out .
	ENTRYPOINT ["dotnet", "DockerDemo.dll"]# 这里只需要修改 DockerDemo.dll的名称

7：安装 docker镜像 
   docker pull mcr.microsoft.com/dotnet/core/sdk:3.1
   docker pull mcr.microsoft.com/dotnet/core/aspnet:3.1
   note:
		这里需要注意两点 生成docker镜像的时候会有 none:none的镜像，这是因为 镜像是分层生成的，
		前一层的镜像还没来得及删除导致的

8：执行 docker build -t xxxx . 
   这里需要注意 xxxx 后面的 . 不应当去掉

9: 运行：
	   docker run -d -p 8080:80 --name myapp xxxx

=====推送镜像 到 dockerhub
docker login 然后输入用户名密码
这里需要注意推送镜像的时候必须 打个tag
docker tag ccnetcore2 15721527020/ccnetcore2
docker push 15721527020/ccnetcore2

下面是具体使用到的一些指令：


