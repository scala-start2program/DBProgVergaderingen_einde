using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scala.Vergaderingen.Core.Entities;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Scala.Vergaderingen.Core.Services
{
    public class VergaderService
    {
        private string ConString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=ScalaVergaderingen; Integrated security=true;";
        public List<Vergadering> GetVergaderingen()
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    List<Vergadering> vergaderingen = connection.GetAll<Vergadering>().ToList();
                    vergaderingen = vergaderingen.OrderBy(p => p.Datum)
                        .ThenBy(p=>p.Van).ToList();
                    return vergaderingen;
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<Vergadering> GetVergaderingen(DateTime? vergaderdatum)
        {
            DateTime datum = (DateTime)vergaderdatum;
            List<Vergadering> vergaderingen = new List<Vergadering>();
            //string sql = "select * from vergaderingen where datum = @datum order by datum, van";
            //using (SqlConnection connection = new SqlConnection(ConString))
            //{
            //    connection.Open();
            //    vergaderingen = connection.Query<Vergadering>(sql, new {datum = datum}).ToList();
            //}
            string sql = "select * from vergaderingen where datum = '" + datum.ToString("yyyy-MM-dd") + "' order by datum, van";
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                connection.Open();
                vergaderingen = connection.Query<Vergadering>(sql).ToList();
            }
            return vergaderingen;

        }
        public List<Persoon> GetPersonen()
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    List<Persoon> personen = connection.GetAll<Persoon>().ToList();
                    personen = personen.OrderBy(p => p.Naam).ToList();
                    return personen;
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<Persoon> GetPersonenInVergadering(Vergadering vergadering)
        {
            List<Persoon> personen = new List<Persoon>();
            string sql;
            sql = "select * from personen p";
            sql += " join deelnemers d on p.id = d.persoonId";
            sql += " where d.vergaderingId = @id";
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                connection.Open();
                personen = connection.Query<Persoon>(sql, vergadering).ToList();
            }
            return personen;
        }
        public bool VergaderingToevoegen(Vergadering vergadering)
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    connection.Insert(vergadering);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool VergaderingWijzigen(Vergadering vergadering)
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    connection.Update(vergadering);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool VergaderingVerwijderen(Vergadering vergadering)
        {
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                VerwijderAlleDeelnemersUitVergadering(vergadering);
                try
                {
                    connection.Open();
                    connection.Delete(vergadering);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        private void VerwijderAlleDeelnemersUitVergadering(Vergadering vergadering)
        {
            string sql = "delete from deelnemers where vergaderingId = @id";
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                connection.Open();
                connection.Execute(sql, vergadering);
            }

        }
        public bool DeelnemerToevoegen(Vergadering vergadering, Persoon persoon)
        {
            Deelnemer deelnemer = new Deelnemer(vergadering.Id, persoon.Id);
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    connection.Insert(deelnemer);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DeelnemerVerwijderen(Vergadering vergadering, Persoon persoon)
        {
            Deelnemer deelnemer = new Deelnemer(vergadering.Id, persoon.Id);
            using (SqlConnection connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    connection.Delete(deelnemer);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
