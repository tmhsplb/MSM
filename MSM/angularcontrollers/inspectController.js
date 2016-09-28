
 
MSMApp.controller('inspectController', ['$scope', '$http', '$q', '$route', 'FileManager','DTOptionsBuilder', 'DTColumnBuilder',
     function ($scope, $http, $q, $route, FileManager, DTOptionsBuilder, DTColumnBuilder) {
       
         $scope.menuFiles = FileManager.getMenuFiles();
        
         $scope.changedValue = function () {
             if ($scope.selectedFile == "Quickbooks") {
                 FileManager.setSelectedFile("Quickbooks");
                 $route.reload();
             }
             else if ($scope.selectedFile == "Apricot") {
                 FileManager.setSelectedFile("Apricot");
                 $route.reload();
             }
             else {
                 alert("FileManager.getQBFileName = " + FileManager.getQBFileName() + " FileManager.getAPFileName() = " + FileManager.getAPFileName());
             }
         }
     }]);