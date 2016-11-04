
FileServices.factory('FileManager', ['$http', function ($http) {
    var getDownloadFile = function (fileName, fileType) {
       // return $http.get("http://localhost/MSM/api/api/download", { params: { "fileName": fileName, "fileType": fileType } }).then(function (result) {
        if (desktop == true) {
            return $http.get("http://localhost/MSM/api/downloadimportme").then(function (result) {
                return result.data;

            })
        }
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

    var setQBFileName = function (name) {
        uploadedFiles.qbFileName = name;
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

    var setAPFileName = function (name) {
        uploadedFiles.apFileName = name;
    }
   
    var getAPFileType = function () {
        return uploadedFiles.apFileType;
    }

    var getVCFileName = function () {
        return uploadedFiles.vcFileName;
    }

    var setVCFileName = function(name) {
        uploadedFiles.vcFileName = name;
    }

    var getVCFileType = function () {
        return uploadedFiles.vcFileType;
    }

    var getMenuFiles = function () {
        return menuFiles;
    }

    var getSelectedFile = function() {
        return uploadedFiles.selectedFile;
    }

    var getValidFile = function(ftype, fileObj)
    {
        var fparts = fileObj.name.split(".");
        var fname = fparts[0];
        var fext = fparts[1];
        if (desktop == true) {
            return $http.get("http://localhost/MSM/api/checkvalidity", { params: { "ftype": ftype, "fname": fname, "fext": fext } }).then(function (result) {
                return result.data;
            })
        } else {
            $http({ method: "GET", url: "https://mymsm.apphb.com/api/checkvalidity", params: { "ftype": ftype, "fname": fname, "fext": fext } }).then(function (result) {
                return result.data;
            })
        }
    }

    
    return {
        getDownloadFile: getDownloadFile,

        getQBFileName: getQBFileName,
        setQBFileName: setQBFileName,
        getQBFileType: getQBFileType,
        setQBUploadFile: setQBUploadFile,

        getAPFileName: getAPFileName,
        setAPFileName: setAPFileName,
        getAPFileType: getAPFileType,
        setAPUploadFile: setAPUploadFile,

        getVCFileName: getVCFileName,
        setVCFileName : setVCFileName,
        getVCFileType: getVCFileType,
        setVCUploadFile: setVCUploadFile,

        getMenuFiles: getMenuFiles,
        setSelectedFile: setSelectedFile,
        getSelectedFile: getSelectedFile,
        getValidFile: getValidFile,
       
    };
}]);

