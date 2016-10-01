
FileServices.factory('FileManager', ['$http', function ($http) {
    var getDownloadFile = function (fileName, fileType) {
        return $http.get("http://localhost/MSM/api/FileUploader/DownloadFile", { params: { "fileName": fileName, "fileType": fileType } }).then(function (result) {
            return result.data;
        })
    };

    var uploadedFiles = {};
    var menuFiles = [];

     
    uploadedFiles.vcFileName = 'unknown';
    uploadedFiles.apFileName = 'unknown';
    uploadedFiles.qbFileName = 'unknown';

    var addToMenuFiles = function (ftype, fname, extension)
    {
        fileName = fname + "." + extension;
        menuFiles.push({type: ftype, file: fileName});
    }

    var setQBUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
        var fname = fparts[0];
        var ftype = fparts[1];
        var qbUploaded = uploadedFiles.qbFileName;

        if (qbUploaded == 'unknown' || qbUploaded != fname) {
            uploadedFiles.qbFileName = fname;
            uploadedFiles.qbFileType = ftype;
            addToMenuFiles("Quickbooks", fname, ftype);
        }
      
    }

    var getQBFileName = function () {
        return uploadedFiles.qbFileName;
    }

    var getQBFileType = function () {
        return uploadedFiles.qbFileType;
    }

    var setAPUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
        var fname = fparts[0];
        var ftype = fparts[1];
        var apUploaded = uploadedFiles.apFileName;

        if (apUploaded == 'unknown' || apUploaded != fname) {
            uploadedFiles.apFileName = fname;
            uploadedFiles.apFileType = ftype;
            addToMenuFiles("Apricot", fname, ftype);
        }
    }

    var setVCUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
        var fname = fparts[0];
        var ftype = fparts[1];
        var vcUploaded = uploadedFiles.vcFileName;

        if (vcUploaded == 'unknown' || vcUploaded != fname) {
            uploadedFiles.vcFileName = fname;
            uploadedFiles.vcFileType = ftype;
            addToMenuFiles("Voidedchecks", fname, ftype);
        }
    }

    var setSelectedFile = function(ftype)
    {
        uploadedFiles.selectedFile = ftype;
    }

    var getAPFileName = function () {
        return uploadedFiles.apFileName;
    }
   
    var getAPFileType = function () {
        return uploadedFiles.apFileType;
    }

    var getVCFileName = function () {
        return uploadedFiles.vcFileName;
    }

    var getVCFileType = function () {
        return uploadedFiles.vcFileType;
    }

    var getMenuFiles = function () {
       // alert("menuFiles = " + menuFiles);
        return menuFiles;
      //  return [{type: "Quickbooks", file: "QB.XLSX"}, {type: "Apricot", file: "AP.XLSX"}];
    }

    var getSelectedFile = function() {
        return uploadedFiles.selectedFile;
    }


    return {
        getDownloadFile: getDownloadFile,
        setQBUploadFile: setQBUploadFile,
        getQBFileName: getQBFileName,
        getQBFileType: getQBFileType,
        setAPUploadFile: setAPUploadFile,
        getAPFileName: getAPFileName,
        getAPFileType: getAPFileType,
        getVCFileName: getVCFileName,
        getVCFileType: getVCFileType,
        setVCUploadFile: setVCUploadFile,
        getMenuFiles: getMenuFiles,
        setSelectedFile: setSelectedFile,
        getSelectedFile: getSelectedFile
    };
}]);

