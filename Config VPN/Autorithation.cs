using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Config_VPN
{
    class Connect
    {
        public static string Server { get; private set; } = "localhost";
        public static string Username { get; private set; } = "root";
        public static string Persistsecurityinfo { get; private set; } = "True";
        public static string Database { get; private set; } = "vpn_users";
        public static string Password { get; private set; } = "0000";

        static string connectionString = "server=" + Server + ";user id=" + Username + ";persistsecurityinfo=True;database=" + Database + ";password=" + Password + ";";
      
        static public void Seting_conection()
        {
            Console.WriteLine("Установка параметров для доступа к базе данных. При вводе пустой строки параметр сохраняется.");
            Console.Write("Сервер: Tекущее значение " + Server + "\tНовое значение: ");

            string value = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(value))
            {
                Server = value;
                Console.WriteLine("Sucsess!");
            }
            Console.Write("UserName: Tекущее значение " + Username + "\tНовое значение: ");
            value = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(value))
            {
                Username = value;
                Console.WriteLine("Sucsess!");
            }
            Console.Write("Database: Tекущее значение " + Database + "\tНовое значение: ");
            value = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(value))
            {
                Database = value;
                Console.WriteLine("Sucsess!");
            }
            Console.Write("Password: Tекущее значение " + Password + "\tНовое значение: ");
            value = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(value))
            {
                Password = value;
                Console.WriteLine("Sucsess!");
            }
            connectionString = "server=" + Server + ";user id=" + Username + ";persistsecurityinfo=True;database=" + Database + ";password=" + Password + ";";
        }

        public static MySqlConnection Connection()
        {
            return new MySqlConnection(connectionString);
        }
    }
    class Autentification
    {
        public delegate void ParsConfHandler(string changes);
        public static event ParsConfHandler Action;
        
        public void Registration()
        {
            MySqlConnection conn = Connect.Connection();
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MySqlCommand command = conn.CreateCommand();
                Console.WriteLine("Регистрация нового пользователя.");
                Console.Write("Введите логин: ");
                string username = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Логин не может быть пустым.");
                    return;
                }
                Console.Write("Введите пароль: ");
                string password = "";

                do
                {
                    ConsoleKeyInfo ski;
                    ski = Console.ReadKey(true);
                    if (ski.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    if (ski.Key == ConsoleKey.Backspace)
                    {
                        Console.Write("\b \b");
                        if (password.Length != 0)
                        {
                            password = password.Remove(password.Length - 1);
                        }
                    }
                    if (ski.KeyChar != '\0' && ski.KeyChar != '\b' && ski.KeyChar != '\t')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("*");
                        password += ski.KeyChar;
                        Console.ResetColor();
                    }

                } while (true);
                Console.WriteLine();
                if (String.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Пароль не может быть пустым.");
                    return;
                }
                command.CommandText = "INSERT INTO `vpn_users`.`user_info` (`user_name`, `user_password`) VALUES ('" + username + "', '" + Hash_SHA512(password) + "');";
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();

                Console.WriteLine("Новый пользователь успешно создан.");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public static bool LogIn(string username, string password)
        {



            MySqlConnection conn = Connect.Connection();
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MySqlCommand command = conn.CreateCommand();


                command.CommandText = "SELECT * FROM user_info WHERE(user_name = '" + username + "') AND(user_password = '" + Hash_SHA512(password) + "')";
                command.CommandType = System.Data.CommandType.Text;
                string name = command.ExecuteScalar()?.ToString();
                if (username == name)
                {
                    Console.WriteLine("Вы вошли в систему.");
                    conn.Close();
                    Action?.Invoke($"Пользователь вошел в систему.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Неверно введен логин или пароль.");
                    Action?.Invoke($"Неудачная попытка войти в систему.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
            finally
            {
                conn.Close();

            }
            return false;
        }
        public static void LogOut()
        {
            Action?.Invoke("Пользователь вышел из системы.");
        }
        private static string Hash_SHA512(string value)
        {
            string hashVslue = "";
            SHA512 sha512 = SHA512.Create();
            byte[] data = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));
            for (int i = 0; i < data.Length; i++)
            {
                hashVslue += (data[i].ToString("x2"));
            }
            return hashVslue;
        }
    }
    class Logs
    {
        public static string User { get; set; }
        public static void Loging_Changes( string change)
        {
            MySqlConnection conn = Connect.Connection();
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO logs (username, Log, DateTime) VALUES('" + User + "', '" + change + "', '" + DateTime.Now + "')";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e);
            }
            finally
            {
                conn.Close();
            }


        }
    }
}
