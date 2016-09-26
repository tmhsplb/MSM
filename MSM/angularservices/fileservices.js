
FileServices.factory('FileManager', ['$http', function ($http) {
    var getDownloadFile = function (fileName, fileType) {
        return $http.get("http://localhost/MSM/api/FileUploader/DownloadFile", { params: { "fileName": fileName, "fileType": fileType } }).then(function (result) {
            return result.data;
        })
    };

    var uploadedFiles = {};

     
    uploadedFiles.apFile = 'unknown';

    uploadedFiles.qbFileName = 'unknown';

    var setQBUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
      //  uploadedFiles.apFileName = 'unknown';
        uploadedFiles.qbFileName = fparts[0];
        uploadedFiles.qbFileType = fparts[1];
      //  alert("setQBUploadFile: uploadedFiles.apFileName = " + uploadedFiles.apFileName);
    }

    var getQBFileName = function () {
        return uploadedFiles.qbFileName;
    }

    var getQBFileType = function () {
        return uploadedFiles.qbFileType;
    }

    var setAPUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
      //  uploadedFiles.qbFileName = 'unknown';
        uploadedFiles.apFileName = fparts[0];
        uploadedFiles.apFileType = fparts[1];
      //  alert("setAPUploadFile: uploadedFiles.qbFileName = " + uploadedFiles.qbFileName);
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

    var getUploadedFiles = function () {
       // alert("Here!");
        return [{type: "Quickbooks", file: "QB.XLSX"}, {type: "Apricot", file: "AP.XLSX"}];
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
        getUploadedFiles: getUploadedFiles,
        setSelectedFile: setSelectedFile,
        getSelectedFile: getSelectedFile
    };
}]);

