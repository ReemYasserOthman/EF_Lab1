using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Ado_Lab1
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;

        public Form1()
        {
            InitializeComponent();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CS"].ConnectionString);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillCoueseData();
            dgv_Courses.Columns[2].Visible = false;
        }

        private void FillCoueseData()
        {
            #region Courses            

            SqlCommand command = new SqlCommand("select Crs_Id,Crs_Name,Crs_Duration,Course.Top_Id,Top_Name from Course  inner join Topic on  Course.Top_Id = Topic.Top_Id", connection);
            //Open connection
            connection.Open();

            //Execute Query
            SqlDataReader dataReader = command.ExecuteReader();

            //prepare data
            List<Course> courses = new List<Course>();

            while (dataReader.Read())
            {
                Course course = new Course();
                course.Id = (int)dataReader[0];
                course.Name = dataReader[1].ToString();               
                if (!Convert.IsDBNull(dataReader[2]))
                {
                    course.Douration = (int)dataReader[2];

                }
                course.Top_Id = (int)dataReader[3];
                course.Top_Name = dataReader[4].ToString();

                
                courses.Add(course);
            }

            //Close connection
            connection.Close();

            dgv_Courses.DataSource = courses;


            cb_Topic.DataSource = FillTopicData();
            cb_Topic.ValueMember = "Id";
            cb_Topic.DisplayMember = "Name";
            
            #endregion

        }
        private List<Topic> FillTopicData()
        {
            #region Topic            

            SqlCommand command = new SqlCommand("select * from Topic", connection);
            //Open connection
            connection.Open();

            //Execute Query
            SqlDataReader dataReader = command.ExecuteReader();

            //prepare data
            List<Topic> topics = new List<Topic>();

            while (dataReader.Read())
            {
                Topic topic = new Topic();
                topic.Id = (int)dataReader[0];
                topic.Name = dataReader[1].ToString();
                topics.Add(topic);
            }

            //Close connection
            connection.Close();

            return topics;

            #endregion
        }
        private int GetLastCourseID()
        {
            // define command
            SqlCommand command = new SqlCommand("select max(Crs_Id) from Course", connection);

            // open connection
            connection.Open();
            // execute command
            var result = command.ExecuteScalar();
            // prepare data
            int id = (int)result;
            //close connection
            connection.Close();

            return id;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            int id = GetLastCourseID() + 100;
            string commandText = "insert into Course values(@id,@name,@duration,@topic)";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("name", text_name.Text);            
            command.Parameters.AddWithValue("duration", nup_Duration.Value);
            command.Parameters.AddWithValue("topic", cb_Topic.SelectedValue);

            // open connection
            connection.Open();
            // execute connection
            int result = command.ExecuteNonQuery();
            // prepare data

            // close connection
            connection.Close();

            if (result > 0)
            {
                MessageBox.Show("Data inserted successfully");
                FillCoueseData();
                ClearInputs();

            }

        }
        private int id = 0;
        
        private void btn_Edit_Click(object sender, EventArgs e)
        {
            string commandText = ("update Course set Crs_Name=@name,Crs_Duration=@duration,Top_Id=@topic where Crs_Id = @id");

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Connection= connection;
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("name", text_name.Text);
            command.Parameters.AddWithValue("duration", nup_Duration.Value);
            command.Parameters.AddWithValue("topic", cb_Topic.SelectedValue);

            //open connection
            connection.Open();

            //execute
            int result = command.ExecuteNonQuery();
            //close connection
            connection.Close();

            if (result > 0)
            {
                MessageBox.Show("Data Updated Successfully");
                FillCoueseData();
                DisplayEditDelete(false);
                ClearInputs();

            }
        }
        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //define command
            SqlCommand commmand = new SqlCommand("delete from Course where Crs_Id=@id", connection);
            commmand.Parameters.Add(new SqlParameter("id", id));

            // open connection
            connection.Open();

            //execute
            int result = commmand.ExecuteNonQuery();

            //close connection
            connection.Close();

            if (result > 0)
            {
                MessageBox.Show("Data deleted successfully");
                FillCoueseData();
                DisplayEditDelete(false);
                ClearInputs();
            }
        }
        private void ClearInputs()
        {
            text_name.Text ="";
            nup_Duration.Value = 0;
        }
        private void DisplayEditDelete(bool visability)
        {
            btn_Delete.Visible = visability;
            btn_Edit.Visible = visability;
            btn_add.Visible = !visability;
        }
        private void dgv_Courses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
        private void dgv_Courses_RowHeaderMouseDoubleClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            DisplayEditDelete(true);
            id = (int)dgv_Courses.SelectedRows[0].Cells[0].Value;
            text_name.Text = dgv_Courses.SelectedRows[0].Cells[1].Value.ToString();
            nup_Duration.Value = (int)dgv_Courses.SelectedRows[0].Cells[2].Value;
            cb_Topic.SelectedValue = (int)dgv_Courses.SelectedRows[0].Cells[3].Value;
        }

        private void dgv_Courses_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
