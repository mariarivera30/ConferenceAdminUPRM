﻿(function () {
    'use strict';

    var controllerId = 'registrationCtrl';

    // TODO: replace app with your module name
    angular.module('app').controller(controllerId,
        ['$scope', '$http', 'restApi', registrationCtrl]);

    function registrationCtrl($scope, $http, restApi) {
        var vm = this;

        //attributes
        vm.activate = activate;

        //add registration fields        
        vm.title = 'registrationCtrl';
        vm.userID;
        vm.paymentID;
        vm.date1;
        vm.date2;
        vm.date3;
        vm.note;
        //vm.checkAll;

        vm.registrationsList = [];
        vm.datesList = [];
        vm.userTypesList = [];
        vm.password;
        vm.email;
        vm.firstname;
        vm.lastname;
        vm.usertypeid;
        vm.affiliationName;
        vm.registrationstatus;
        vm.hasapplied;
        vm.acceptancestatus;
        vm.byAdmin;

        //for pagination
        vm.sindex = 0;
        vm.smaxIndex = 0;
        vm.sfirstPage = true;

        vm.currentid;
        vm.editfirstname;
        vm.editlastname;
        vm.editusertypeid;
        vm.editaffiliationName;
        vm.editnote;

        // Functions
        vm.activate = activate;
        vm.addRegistration = _addRegistration;
        vm.getRegistrations = _getRegistrations;
        vm.searchRegistration = _searchRegistration;
        vm.nextRegistration = _nextRegistration;
        vm.previousRegistration = _previousRegistration;
        vm.getFirstRegistrationPage = _getFirstRegistrationPage;
        vm.getLastRegistrationPage = _getLastRegistrationPage;
        vm.updateRegistration = _updateRegistration;
        vm.deleteRegistration = _deleteRegistration;
        vm.selectedRegistrationUpdate = _selectedRegistrationUpdate;
        vm.selectedRegistrationDelete = _selectedRegistrationDelete;
        vm.getUserTypes = _getUserTypes;
        vm.getDates = _getDates;
        vm.clear = clear;
        vm.downloadAttendanceList = _downloadAttendanceList;


        _getRegistrations(vm.sindex);
        _getUserTypes();
        _getDates();



        // Functions
        function activate() {
            firstname = vm.firstname;
            lastname = vm.lastname;
            usertypeid = vm.usertypeid;
            affiliationName = vm.affiliationName;
            registrationstatus = vm.registrationstatus;
            hasapplied = vm.hasapplied;
            acceptancestatus = vm.acceptancestatus;
            date1 = vm.date1;
            date2 = vm.date2;
            date3 = vm.date3;
        }

        /* [Randy] resets all attributes */
        function clear() {
            vm.password = "";
            vm.email = "";
            vm.firstname = "";
            vm.lastname = "";
            vm.usertypeid = "";
            vm.affiliationName = "";
            vm.title = "";
            vm.registrationstatus = "";
            vm.hasapplied = false;
            vm.acceptancestatus = "";
            vm.date1 = false;
            vm.date2 = false;
            vm.date3 = false;
            vm.note = "";
            //vm.checkAll = false;
            vm.currentid = 0;
            vm.editfirstname = "";
            vm.editlastname = "";
            vm.editusertypeid = "";
            vm.editaffiliationName = "";
            vm.edittitle = "";
            vm.editregistrationstatus = "";
            vm.edithasapplied = false;
            vm.editacceptancestatus = "";
            vm.editdate1 = false;
            vm.editdate2 = false;
            vm.editdate3 = false;
            vm.editnote = "";
            $scope.content = "";
            $scope.$fileContent = "";
        }

        /* [Randy] add new registration entry */
        function _addRegistration() {
            var userTypeName = vm.usertypeid.userTypeName;
            vm.usertypeid = vm.usertypeid.userTypeID;
            vm.processing = true;

            restApi.postNewRegistration(vm)
                .success(function (data, status, headers, config) {
                    /*vm.newReg = { registrationID: parseInt(data.split(",")[0]), firstname: vm.firstname, lastname: vm.lastname, affiliationName: vm.affiliationName, usertypeid: userTypeName, date1: vm.date1, date2: vm.date2, date3: vm.date3, byAdmin: true, notes: vm.note };
                    vm.registrationsList.push(vm.newReg);
                    clear();
                    vm.recoverType = parseInt(data.split(",")[1]);
                    vm.recoverID = parseInt(data.split(",")[0]);*/                    
                    _getRegistrations(vm.sindex);
                    vm.processing = false;
                    $('#register').modal('hide');
                })

                .error(function (error) {
                    alert("Failed to add Registration.");
                });

            //activate();
        }

        /* [Randy] get list of all registration entries */
        function _getRegistrations(index) {
            restApi.getRegistrations(index).
                   success(function (data, status, headers, config) {
                       if (data.results == null)
                           vm.empty = true;
                       vm.smaxIndex = data.maxIndex;
                       if (vm.smaxIndex == 0) {
                           vm.sindex = 0;
                           vm.registrationsList = [];
                       }
                       else if (vm.sindex >= vm.smaxIndex) {
                           vm.sindex = vm.smaxIndex - 1;
                           _getRegistrations(vm.sindex);
                       }
                       else {
                           vm.registrationsList = data.results;
                       }
                   }).
                   error(function (data, status, headers, config) {
                   });
        }

        /* [Jaimeiris] Pagination: get next page */
        function _nextRegistration() {
            if (vm.sindex < vm.smaxIndex - 1) {
                vm.sindex += 1;
                _getRegistrations(vm.sindex);
            }
        }

        /* [Jaimeiris] Pagination: get previous page */
        function _previousRegistration() {
            if (vm.sindex > 0) {
                vm.sindex -= 1;
                _getRegistrations(vm.sindex);
            }
        }

        /* [Jaimeiris] Pagination: get first page */
        function _getFirstRegistrationPage() {
            vm.sindex = 0;
            _getRegistrations(vm.sindex);
        }

        /* [Jaimeiris] Pagination: get last page */
        function _getLastRegistrationPage() {
            vm.sindex = vm.smaxIndex - 1;
            _getRegistrations(vm.sindex);
        }
        //----END PAGINATON CODE---


        /* [Randy] prepare registration to be edited */
        function _selectedRegistrationUpdate(id, firstname, lastname, usertypeid, affiliationName, date1, date2, date3, note) {
            vm.currentid = id;
            vm.editfirstname = firstname;
            vm.editlastname = lastname;
            if (usertypeid != undefined)
                vm.TYPE = vm.userTypesList[usertypeid.userTypeID - 1];
            if (id == vm.recoverID) {
                vm.TYPE = vm.userTypesList[vm.recoverType - 1];
            }
            vm.editaffiliationName = affiliationName;
            vm.editdate1 = date1;
            vm.editdate2 = date2;
            vm.editdate3 = date3;
            vm.editnote = note;
        }

        /* [Randy] edit a specific registration entry */
        function _updateRegistration() {
            /*if (vm.checkAll) {
                vm.editdate1 = true;
                vm.editdate2 = true;
                vm.editdate3 = true;
            }*/
            if (vm.currentid != undefined && vm.currentid != 0) {
                var registration = { registrationID: vm.currentid, firstname: vm.editfirstname, lastname: vm.editlastname, usertypeid: vm.TYPE.userTypeID, affiliationName: vm.editaffiliationName, date1: vm.editdate1, date2: vm.editdate2, date3: vm.editdate3, notes: vm.editnote };
                restApi.updateRegistration(registration)
                .success(function (data, status, headers, config) {
                    vm.registrationsList.forEach(function (reg, index) {
                        if (reg.registrationID == registration.registrationID) {
                            $('#editAttendee').modal('hide');
                            registration.usertypeid = vm.TYPE.userTypeName;
                            registration.byAdmin = true;
                            vm.registrationsList.splice(index, 1);
                            vm.registrationsList.push(registration);        
                            clear();
                        }
                        // else
                        // location.reload();
                    });
                })
                .error(function (data, status, headers, config) {
                    alert("Failed to edit Registration.");
                    location.reload();
                });
            }
            else {
                alert("There is information missing.");
            }
            clear();
        }
        
        /* [Randy] alert of deletion, prepare the registration to be deleted */
        function _selectedRegistrationDelete(id) {
            vm.currentid = id;
        }

        /* [Randy] delete a specific registration entry */
        function _deleteRegistration() {
            if (vm.currentid != undefined) {
                restApi.deleteRegistration(vm.currentid)
                .success(function (data, status, headers, config) {
                    vm.registrationsList.forEach(function (registration, index) {
                        if (registration.registrationID == vm.currentid) {
                            vm.registrationsList.splice(index, 1);
                        }
                    });
                })
                .error(function (data, status, headers, config) {
                });
            }
        }

        /* [Randy] get list of all types of users */
        function _getUserTypes() {
            restApi.getUserTypes().
                   success(function (data, status, headers, config) {
                       vm.userTypesList = data;
                   }).
                   error(function (data, status, headers, config) {
                       vm.userTypesList = data;
                   });
        }

        /* [Randy] get dates of the conference */
        function _getDates() {
            restApi.getDates().
                   success(function (data, status, headers, config) {
                       vm.datesList = data;
                   }).
                   error(function (data, status, headers, config) {
                       vm.datesList = data;
                   });
        }

        /* [Heidi] download report of attendees */
        function _downloadAttendanceList() {
            //vm.loading = true;
            //vm.downloadLoading = true;
            restApi.getAttendanceReport()
            .success(function (data, status, headers, config) {
                if (data != null && data != "") {
                    var report = data.results;
                    //vm.downloadLoading = false;
                    //vm.loading = false;

                    if (report != "" && report != undefined) {
                        var blob = new Blob([report], { type: "text/plain;charset=utf-8" });
                        saveAs(blob, "Attendance_Report.csv");
                    }
                }
            })
           .error(function (data, status, headers, config) {
               //vm.loading = false;
               //vm.downloadLoading = false;
           });
        }

        /* [Randy] search for a registration using a certain criteria */
        function _searchRegistration() {
            vm.sindex = 0;
            var params = { index: vm.sindex, criteria: vm.criteria };
            restApi.searchRegistration(params).
                   success(function (data, status, headers, config) {
                       vm.smaxIndex = data.maxIndex;
                       if (vm.smaxIndex == 0) {
                           vm.sindex = 0;
                           vm.registrationsList = [];
                       }
                       else if (vm.sindex >= vm.smaxIndex) {
                           vm.sindex = vm.smaxIndex - 1;
                           _getRegistrations(vm.sindex);
                       }
                       else {
                           vm.registrationsList = data.results;
                       }
                   }).
                   error(function (data, status, headers, config) {
                   });
        }
    }
})();
