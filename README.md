# EasyChat

本程序实现了MQTT在局域网中不同客户端互相通讯的功能。

> 优点：

1. 无需连接外网，适合车间调试工程师；
2. 没有数据库，不会存储聊天记录，不会被公司电脑监控；
3. 使用AES对称加密算法加密消息体，再也不怕抓包了，抓包的人肯定想不到我把秘钥写死在代码里了。

> 缺点：

1. 仅支持发送字符串，不能发文件、图片、表情包；
2. 当前仅支持Windows系统。

> 待办项：

1. 程序运行内存优化（已经干到200多M了）

