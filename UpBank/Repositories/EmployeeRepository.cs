using Models.DTO;
using Models.People;
using System.Data.SqlClient;
using Dapper;

namespace Repositories
{
    public class EmployeeRepository
    {
        public bool Post(Employee employee)
        {
            string strConn = "Data Source=127.0.0.1; Initial Catalog=DBEmployee; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";

            using (var connection = new SqlConnection(strConn))
            {
                connection.Open();

                string insertEmployeeQuery = @"INSERT INTO Employee (Name, CPF, BirthDt, Sex, IdAddress, Salary, Phone, Email, Manager, Registry) 
                                                       VALUES (@Name, @CPF, @BirthDt, @Sex, @IdAddress, @Salary, @Phone, @Email, @Manager, @Registry);";

                object obj = new
                {
                    Name = employee.Name,
                    CPF = employee.CPF,
                    BirthDt = employee.BirthDt,
                    Sex = employee.Sex,
                    IdAddress = employee.Address,
                    Salary = employee.Salary,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    Manager = employee.Manager,
                    Registry = employee.Registry
                };
                return connection.Execute(insertEmployeeQuery, obj) > 0;    
            }
        }

        public void Delete(int registry)
        {
            string strConn = "Data Source=127.0.0.1; Initial Catalog=DBEmployee; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";
        }
    }
}
