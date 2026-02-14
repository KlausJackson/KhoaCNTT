using BCrypt.Net;
string a = "$2a$11$apTVlOWXAO4UPjsUXyNGg.udEKcsSFtV2UDy07beFEpCxvvmmdNoS";
string b = BCrypt.Net.BCrypt.HashPassword("123");

bool c = a == b;
Console.WriteLine(c);
