鸿瑞云视频pispower-csharp-sdk
===================
>版权所有（C）广州鸿瑞网络有限公司

>https://video.cloudak47.com

环境要求
-------------
>.NET Framework及以上版本
>必须注册有鸿瑞云视频帐号(登录-服务设置-RestfulAPI支持,获取AccessKey,AccessSecret)

提供了以下功能
-------------
>视频目录:对视频目录的操作
>视频:对视频的操作
>断点续传:支持对视频的断点续传

组织结构
-------------
```
pispower-csharp-sdk
   |
   |---src
   |    |
   |    |---Catalog   					catalog操作源代码
   |    |
   |    |---Internal   					内部访问源代码
   |    |
   |    |---Video      					video操作源代码
   |          |
   |          |---multipartupload   	断点续传源代码
   |          |
   |          |---others				video操作
   |
   |---Demo			   					示例源代码
```

详细参考文档
-------------
>开发者支持-RestfulAPI

联系我们
-------------
>客服热线：400-668-1010
>技术支持：support.video@onecloud.cn