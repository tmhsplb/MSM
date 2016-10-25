MSMApp.controller('datatableController', ['$scope', '$http', '$q', '$filter', 'FileManager', 'MergeManager', 'DTOptionsBuilder', 'DTColumnBuilder',
     function ($scope, $http, $q, $filter, FileManager, MergeManager, DTOptionsBuilder, DTColumnBuilder) {

         var vm = this;
         var someResolved  = false;
         $scope.pleaseAct = false;
        
         if ($scope.tab == 'inspect' && FileManager.getSelectedFile() == "Quickbooks") {
            // alert("datatableController fname = " + FileManager.getQBFileName() + " ftype = " + FileManager.getQBFileType());
 
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/qbfile',
                    { params: { "qbFile": FileManager.getQBFileName(), "fileType": FileManager.getQBFileType() } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
             vm.dtColumns = [
                     DTColumnBuilder.newColumn('Date').withTitle('Date').renderWith(function (data, type) {
                         return $filter('date')(data, 'dd/MM/yyyy')
                     }), 
                     DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                     DTColumnBuilder.newColumn('Memo').withTitle('Memo'),
                     DTColumnBuilder.newColumn('Clr').withTitle('Status')
                 ];
         }
         else if ($scope.tab == 'inspect' && FileManager.getSelectedFile() == "Voidedchecks")
         {
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/vcfile', { params: { "vcFile": FileManager.getVCFileName(), "fileType": FileManager.getVCFileType() } }).then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers');
             vm.dtColumns = [
                 DTColumnBuilder.newColumn('Date').withTitle('Date').renderWith(function (data, type) {
                     return $filter('date')(data, 'dd/MM/yyyy')
                 }),
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
                 DTColumnBuilder.newColumn('InterviewRecordID').withTitle('Interview Record ID'),
                 DTColumnBuilder.newColumn('Lname').withTitle("Last Name"),
                 DTColumnBuilder.newColumn('Fname').withTitle("First Name"),
                 DTColumnBuilder.newColumn('LBVDCheckNum').withTitle('LBVD Check Number'),
                 DTColumnBuilder.newColumn('LBVDCheckDisposition').withTitle('LBVD Check Disposition'),
                 DTColumnBuilder.newColumn('TIDCheckNum').withTitle('TID Check Number'),
                 DTColumnBuilder.newColumn('TIDCheckDisposition').withTitle('TID Check Disposition'),
                 DTColumnBuilder.newColumn('TDLCheckNum').withTitle('TDL Check Number'),
                 DTColumnBuilder.newColumn('TDLCheckDisposition').withTitle('TDL Check Disposition'),
                 DTColumnBuilder.newColumn('MBVDCheckNum').withTitle('MBVD Check Number'),
                 DTColumnBuilder.newColumn('MBVDCheckDisposition').withTitle('MBVD Check Disposition'),
                 DTColumnBuilder.newColumn('SDCheckNum').withTitle('SD Check Number'),
                 DTColumnBuilder.newColumn('SDCheckDisposition').withTitle('SD Check Disposition')
             ];
         }
         else if ($scope.tab == 'resolved') {
             DisplayResolvedChecks();
         }

         else if ($scope.tab == 'research') {
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/research').then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers')
             .withButtons(['print']);

             vm.dtColumns = [
                 DTColumnBuilder.newColumn('Date').withTitle('Date').renderWith(function (data, type) {
                     return $filter('date')(data, 'dd/MM/yyyy')
                 }),
                 DTColumnBuilder.newColumn('RecordID').withTitle('Record ID'),
                 DTColumnBuilder.newColumn('InterviewRecordID').withTitle('Interview Record ID'),
                 DTColumnBuilder.newColumn('Name').withTitle('Name'),
                 DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                 DTColumnBuilder.newColumn('Service').withTitle('Service')
             ];
         }
         else { // If this final "else" clause is removed a controller error will occur. Do not remove!
             // alert("Load the empty file to avoid a controller error");
             LoadTheEmptyFile();
         }

         FileManager.setSelectedFile("Empty");

         function DisplayResolvedChecks() {  // There not be any. This is handled by returning an empty list of checks from the API.
             vm.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
                 var defer = $q.defer();
                 $http.get('http://localhost/msm/api/resolved').then(function (result) {
                     defer.resolve(result.data);
                 });
                 return defer.promise;
             }).withPaginationType('full_numbers')
               .withButtons(['print']);
             

             vm.dtColumns = [
                      DTColumnBuilder.newColumn('Date').withTitle('Date').renderWith(function (data, type) {
                          return $filter('date')(data, 'dd/MM/yyyy')
                      }),
                      DTColumnBuilder.newColumn('RecordID').withTitle('Record ID'),
                      DTColumnBuilder.newColumn('InterviewRecordID').withTitle('Interview Record ID'),
                      DTColumnBuilder.newColumn('Name').withTitle('Name'),
                      DTColumnBuilder.newColumn('Num').withTitle('Check Number'),
                      DTColumnBuilder.newColumn('Service').withTitle('Service'),
                      DTColumnBuilder.newColumn('Clr').withTitle('Status')
             ];
         }

         function LoadTheEmptyFile()
         {
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
     }
])