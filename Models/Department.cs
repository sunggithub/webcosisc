using System.ComponentModel.DataAnnotations;

namespace Website.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public int DepartmentStrucId { get; set; }
    public string DepartmentJobTitle { get; set; }
    public string DepartmentName { get; set; }
    public string DepartmentPhone { get; set; }
    public string DepartmentEmail { get; set; }
    public string DepartmentImage { get; set; }
    public string DepartmentDescrip { get; set; }

    public Department()
    {
        if (DepartmentJobTitle == null)
        {
            DepartmentJobTitle = "";
        }
        if (DepartmentName == null)
        {
            DepartmentName = "";
        }
        if (DepartmentPhone == null)
        {
            DepartmentPhone = "";
        }
        if (DepartmentEmail == null)
        {
            DepartmentEmail = "";
        }
        if (DepartmentImage == null)
        {
            DepartmentImage = "";
        }
        if (DepartmentDescrip == null)
        {
            DepartmentDescrip = "";
        }
    }

}

