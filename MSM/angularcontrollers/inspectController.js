
 
MSMApp.controller('inspectController', ['$rootScope', '$scope', '$http', '$q', '$route', 'FileManager',
     function ($rootScope, $scope, $http, $q, $route, FileManager) {
         $rootScope.pageTitle = "Main Street Ministries - Inspect";
         $scope.menuFiles = FileManager.getMenuFiles();
         $scope.tab = 'inspect'
        
         $scope.changedValue = function () {
             if ($scope.selectedFile == "Quickbooks") {
                 FileManager.setSelectedFile("Quickbooks");
                 $route.reload();
             }
             else if ($scope.selectedFile == "Apricot") {
                 FileManager.setSelectedFile("Apricot");
                 $route.reload();
             }
             else if ($scope.selectedFile == "Voidedchecks") {
                 FileManager.setSelectedFile("Voidedchecks");
                 $route.reload();
             }
             else {
                 alert("FileManager.getQBFileName = " + FileManager.getQBFileName() + " FileManager.getAPFileName() = " + FileManager.getAPFileName());
             }
         }
     }]);