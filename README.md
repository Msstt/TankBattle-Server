# 坦克大战服务端

客户端：https://github.com/Msstt/TankBattle-Client

## 部署方式

1. 登录 mysql

```mysql
CREATE DATABASE tank_battle;
USE DATABASE tank_battle;
CREATE TABLE account {
  id VARCHAR(20) PRIMARY KEY,
  password TEXT
};
CREATE TABLE player {
  id VARCHAR(20) PRIMARY KEY,
  data TEXT
};
```

2. 命令行运行服务器，需要 4 个参数
   - 服务端监听的 IP 地址
   - 服务端监听的端口号
   - mysql 登录账号
   - mysql 登录密码

## 功能

- 基于 Select 实现网络模块，使用反射特性实现消息订阅，事件订阅功能
- 连接 mysql 数据库完成账号、战绩信息的可持久化