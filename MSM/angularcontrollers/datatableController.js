MSMApp.controller('datatableController', ['$scope', '$http', '$q', 'FileManager', 'MergeManager', 'DTOptionsBuilder', 'DTColumnBuilder',
     function ($scope, $http, $q, FileManager, MergeManager, DTOptionsBuilder, DTColumnBuilder) {

         var vm = this;
         $scope.pleaseAct = false;
        

         if ($scope.tab == 'inspect' && FileManager.getSelectedFile() == "Quickbooks") {
            // alert("datatableController fname = " + FileManager.getQBFileName() + " ftype = " + FileManager.getQBFileType());
 
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/qbfile', { params: { "quickbooksFile": FileManager.getQBFileName(), "fileType": FileManager.getQBFileType() } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
                 vm.dtColumns = [
                     DTColumnBuilder.newColumn('Date').withTitle('Date'),
                     DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                     DTColumnBuilder.newColumn('Memo').withTitle('Memo'),
                   //  DTColumnBuilder.newColumn('Service').withTitle('Service').notVisible(),
                     DTColumnBuilder.newColumn('Clr').withTitle('Status'),
                   //  DTColumnBuilder.newColumn('Type').withTitle('Type').notVisible(),
                   //  DTColumnBuilder.newColumn('InterviewRecordID').withTitle('Record ID').notVisible()
                 ];
         }
         else if ($scope.tab == 'inspect' && FileManager.getSelectedFile() == "Voidedchecks")
         {
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/vcfile', { params: { "voidedchecksFile": FileManager.getVCFileName(), "fileType": FileManager.getVCFileType() } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
             vm.dtColumns = [
                 DTColumnBuilder.newColumn('Date').withTitle('Date'),
                 DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                 DTColumnBuilder.newColumn('Memo').withTitle('Memo')
             ];
         }
         else if ($scope.tab == 'inspect' && FileManager.getSelectedFile() == "Apricot")
         {
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/apfile', { params: { "apricotFile": FileManager.getAPFileName(), "fileType": FileManager.getAPFileType() } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');

             vm.dtColumns = [
                 DTColumnBuilder.newColumn('RecordID').withTitle('Record ID'),
                 DTColumnBuilder.newColumn('LBVDCheckNum').withTitle('LBVD Check Number'),
                 DTColumnBuilder.newColumn('LBVDCheckDisposition').withTitle('LBVD Check Disposition'),
                 DTColumnBuilder.newColumn('TIDCheckNum').withTitle('TID Check Number'),
                 DTColumnBuilder.newColumn('TIDCheckDisposition').withTitle('TID Check Disposition'),
                 DTColumnBuilder.newColumn('TDLCheckNum').withTitle('TDL Check Number'),
                 DTColumnBuilder.newColumn('TDLCheckDisposition').withTitle('TDL Check Disposition'),
                 DTColumnBuilder.newColumn('MBVDCheckNum').withTitle('MBVD Check Number'),
                 DTColumnBuilder.newColumn('MBVDCheckDisposition').withTitle('MBVD Check Disposition'),
                 DTColumnBuilder.newColumn('SDCheckDisposition').withTitle('SD Check Disposition')
             ];
             ];
         }
         else if ($scope.tab == 'review' && MergeManager.performedMerge() == true)
         {
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/unmatched', { params: { "recent": true } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
             vm.dtColumns = [
                 DTColumnBuilder.newColumn('InterviewRecordID').withTitle('Interview Record ID'),
                 DTColumnBuilder.newColumn('Date').withTitle('Date'),
                 DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                
                 DTColumnBuilder.newColumn('Service').withTitle('Service'),
                 DTColumnBuilder.newColumn('Type').withTitle('Type')
             ];
         }
         else
         {
            // alert("Load the empty file to avoid a controller error");
             $scope.pleaseAct = true;
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/emptyfile', { params: { "emptyFile": "Empty", "fileType": "XLSX" } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
             vm.dtColumns = [
                 DTColumnBuilder.newColumn('Empty').withTitle('Empty'),
             ];
         }

         FileManager.setSelectedFile("Empty");
     }
]);