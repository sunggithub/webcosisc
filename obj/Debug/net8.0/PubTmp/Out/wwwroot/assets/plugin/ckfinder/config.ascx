<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="CKFinder.Settings.ConfigFile" %>
<%@ Import Namespace="CKFinder.Settings" %>
<script runat="server">

    /**
	 * this function must check the user session to be sure that he/she is
	 * authorized to upload and access files using ckfinder.
	 */
    // đây là chỗ kiểm tra quyền trước khi cho hiện ckfinder
    public override bool checkauthentication()
    {
        return true;
    }

    /**
	 * all configuration settings must be defined here.
	 */
    public override void setconfig()
    {
        // paste your license name and key here. if left blank, ckfinder will
        // be fully functional, in demo mode.

        // đây là licent 
        licensename = "localhost";
        licensekey = "5QN1X65XB27MF24H5LKBAPEAH8NRP1CY";


        // đây là thiết lập đường dẫn lưu file
        baseurl = "/assets";

        // the phisical directory in the server where the file will end up. if
        // blank, ckfinder attempts to resolve baseurl.
        basedir = httpcontext.current.server.mappath("/assets");


        // optional: enable extra plugins (remember to copy .dll files first).
        plugins = new string[] {
			//"ckfinder.plugins.fileeditor, ckfinder_fileeditor",
			 //"ckfinder.plugins.imageresize, ckfinder_imageresize",
			 //"ckfinder.plugins.watermark, ckfinder_watermark"
		};
        // thiết lập các loại ảnh được tạo ra thêm khi mình upload ảnh lên

        pluginsettings = new hashtable();
        pluginsettings.add("imageresize_smallthumb", "90x90");
        pluginsettings.add("imageresize_mediumthumb", "120x120");
        pluginsettings.add("imageresize_largethumb", "180x180");
        // name of the watermark image in plugins/watermark folder
        pluginsettings.add("watermark_source", "logo.gif");
        pluginsettings.add("watermark_marginright", "5");
        pluginsettings.add("watermark_marginbottom", "5");
        pluginsettings.add("watermark_quality", "90");
        pluginsettings.add("watermark_transparency", "80");

        // thumbnail settings.
        // "url" is used to reach the thumbnails with the browser, while "dir"
        // points to the physical location of the thumbnail files in the server.

        // đây là đường dẫn ảnh nhỏ - sử dụng trong quá trình hiển thị
        //thumbnails.url = baseurl + "/thumbs/";
        //if ( basedir != "" ) {
        //    thumbnails.dir = basedir + "/thumbs/";
        //}
        //thumbnails.enabled = true;
        //thumbnails.directaccess = false;
        //thumbnails.maxwidth = 100;
        //thumbnails.maxheight = 100;
        //thumbnails.quality = 80;

        // set the maximum size of uploaded images. if an uploaded image is
        // larger, it gets scaled down proportionally. set to 0 to disable this
        // feature.
        images.maxwidth = 1600;
        images.maxheight = 1200;
        images.quality = 80;

        // indicates that the file size (maxsize) for images must be checked only
        // after scaling them. otherwise, it is checked right after uploading.
        checksizeafterscaling = true;

        // increases the security on an iis web server.
        // if enabled, ckfinder will disallow creating folders and uploading files whose names contain characters
        // that are not safe under an iis 6.0 web server.
        disallowunsafecharacters = true;

        // if checkdoubleextension is enabled, each part of the file name after a dot is
        // checked, not only the last part. in this way, uploading foo.php.rar would be
        // denied, because "php" is on the denied extensions list.
        // this option is used only if forcesingleextension is set to false.
        checkdoubleextension = true;

        // due to security issues with apache modules, it is recommended to leave the
        // following setting enabled. it can be safely disabled on iis.
        forcesingleextension = true;

        // for security, html is allowed in the first kb of data for files having the
        // following extensions only.
        htmlextensions = new string[] { "html", "htm", "xml", "js" };

        // folders to not display in ckfinder, no matter their location. no
        // paths are accepted, only the folder name.
        // the * and ? wildcards are accepted.
        // by default folders starting with a dot character are disallowed.
        hidefolders = new string[] { ".*", "cvs" };

        // files to not display in ckfinder, no matter their location. no
        // paths are accepted, only the file name, including extension.
        // the * and ? wildcards are accepted.
        hidefiles = new string[] { ".*" };

        // perform additional checks for image files.
        secureimageuploads = true;

        // the session variable name that ckfinder must use to retrieve the
        // "role" of the current user. the "role" is optional and can be used
        // in the "accesscontrol" settings (bellow in this file).
        rolesessionvar = "ckfinder_userrole";

        // acl (access control) settings. used to restrict access or features
        // to specific folders.
        // several "accesscontrol.add()" calls can be made, which return a
        // single acl setting object to be configured. all properties settings
        // are optional in that object.
        // subfolders inherit their default settings from their parents' definitions.
        //
        //	- the "role" property accepts the special "*" value, which means
        //	  "everybody".
        //	- the "resourcetype" attribute accepts the special value "*", which
        //	  means "all resource types".
        accesscontrol acl = accesscontrol.add();
        acl.role = "*";
        acl.resourcetype = "*";
        acl.folder = "/";

        acl.folderview = true;
        acl.foldercreate = true;
        acl.folderrename = true;
        acl.folderdelete = true;

        acl.fileview = true;
        acl.fileupload = true;
        acl.filerename = true;
        acl.filedelete = true;

        // resource type settings.
        // a resource type is nothing more than a way to group files under
        // different paths, each one having different configuration settings.
        // each resource type name must be unique.
        // when loading ckfinder, the "type" querystring parameter can be used
        // to display a specific type only. if "type" is omitted in the url,
        // the "defaultresourcetypes" settings is used (may contain the
        // resource type names separated by a comma). if left empty, all types
        // are loaded.

        // ==============================================================================
        // attention: flash files with `swf' extension, just like html files, can be used
        // to execute javascript code and to e.g. perform an xss attack. grant permission
        // to upload `.swf` files only if you understand and can accept this risk.
        // ==============================================================================

        // thiết lập các loại file có thể tải lên server

        defaultresourcetypes = "";

        resourcetype type;
        // loại 1: hình ảnh, cho vào thư mục images (thư mục này tự được tạo khi chạy ckfinder lần đầu)

        type = resourcetype.add("image");
        type.url = baseurl + "/image/";
        type.dir = basedir == "" ? "" : basedir + "/image/";
        type.maxsize = 0;
        //các định dạng file có thể tải vào thư mục images
        type.allowedextensions = new string[] { "bmp", "gif", "jpeg", "jpg", "png" };
        type.deniedextensions = new string[] { };

        // thư mục 2: lưu tất cả các loại file
        resourcetype type2;
        type2 = resourcetype.add("files");
        type2.url = baseurl + "/files/";
        type2.dir = basedir == "" ? "" : basedir + "/files/";
        type2.maxsize = 0;
        // để string rỗng => file gì tải lên cũng được
        type2.allowedextensions = new string[] { };
        type2.deniedextensions = new string[] { };

    }

</script>
