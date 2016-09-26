
 
MSMApp.controller('inspectController', ['$scope', '$http', '$q', '$route', 'FileManager','DTOptionsBuilder', 'DTColumnBuilder',
     function ($scope, $http, $q, $route, FileManager, DTOptionsBuilder, DTColumnBuilder) {
       
         $scope.uploadedFiles = FileManager.getUploadedFiles();
        
         $scope.changedValue = function () {
             if ($scope.selectedFile == "Quickbooks") {
                 FileManager.setSelectedFile("Quickbooks");
             //    alert("QB inspectController fname = " + FileManager.getQBFileName() + " ftype = " + FileManager.getQBFileType());
             //    alert("Call $route.reload()");
                 $route.reload();
             }
             else if ($scope.selectedFile == "Apricot") {
               //  alert("AP inspectController fname = " + FileManager.getAPFileName() + " ftype = " + FileManager.getAPFileType());
                 FileManager.setSelectedFile("Apricot");
                 $route.reload();
             }
             else {
                 alert("FileManager.getQBFileName = " + FileManager.getQBFileName() + " FileManager.getAPFileName() = " + FileManager.getAPFileName());
             }
         }
     }]);