﻿
FileServices.factory('FileManager', ['$http', function ($http) {
    var getDownloadFile = function (fileName, fileType) {
       // return $http.get("http://localhost/MSM/api/api/download", { params: { "fileName": fileName, "fileType": fileType } }).then(function (result) {
       return $http.get(server + "api/downloadimportme").then(function (result) {
            return result.data;
        })
    };

    var uploadedFiles = {};
    var menuFiles = [];

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

        if (qbUploaded == undefined || qbUploaded != fname) {
            uploadedFiles.qbFileName = fname;
            uploadedFiles.qbFileType = ftype;
            addToMenuFiles("Quickbooks", fname, ftype);
        }
    }

    var getQBFileName = function () {
        if (uploadedFiles.qbFileName == undefined)
        {
            return "unknown";
        }

        return uploadedFiles.qbFileName;
    }

    var setQBFileName = function (name) {
        uploadedFiles.qbFileName = name;
    }

    var getQBFileType = function () {
        if (uploadedFiles.qbFileType == undefined)
        {
            return "";
        }

        return uploadedFiles.qbFileType;
    }

    var setAPUploadFile = function (fileObj) {
        var fparts = fileObj.name.split(".");
        var fname = fparts[0];
        var ftype = fparts[1];
        var apUploaded = uploadedFiles.apFileName;

        if (apUploaded == undefined || apUploaded != fname) {
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

        if (vcUploaded == undefined || vcUploaded != fname) {
            uploadedFiles.vcFileName = fname;
            uploadedFiles.vcFileType = ftype;
            addToMenuFiles("Voidedchecks", fname, ftype);
        }
    }

    var setSelectedFile = function(fname)
    {
        uploadedFiles.selectedFile = fname;
    }

    var getAPFileName = function () {
        if (uploadedFiles.apFileName == undefined) {
            return "unknown";
        };

        return uploadedFiles.apFileName;
    }

    var setAPFileName = function (name) {
        uploadedFiles.apFileName = name;
    }
   
    var getAPFileType = function () {
        if (uploadedFiles.apFileType == undefined)
        {
            return "";
        }
        return uploadedFiles.apFileType;
    }

    var getVCFileName = function () {
        if (uploadedFiles.vcFileName == undefined)
        {
            return "unknown";
        }

        return uploadedFiles.vcFileName;
    }

    var setVCFileName = function(name) {
        uploadedFiles.vcFileName = name;
    }

    var getVCFileType = function () {
        if (uploadedFiles.vcFileType == undefined)
        {
            return "";
        }
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
        return $http.get(server + "api/checkvalidity",
            { params: { "ftype": ftype, "fname": fname, "fext": fext } }).then(function (result) {
                return result.data;
            })
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

