﻿(function () {
    'use strict';

    var controllerId = 'guestCtrl';

    // TODO: replace app with your module name
    angular.module('app').controller(controllerId,
        ['$scope', '$http', 'restApi', guestCtrl]);

    function guestCtrl($scope, $http, restApi) {
        var vm = this;
        vm.activate = activate;
        //object fields
        vm.title = 'guestCtrl';
        vm.userID;
        vm.firstName;
        vm.lastName;
        vm.userType;
        vm.acceptanceStatus;
        vm.authorizationStatus;
        vm.photoAuthorization;
        vm.overnightAuthorization;
        vm.companionAuthorization;
        vm.participationAuthorization;
        vm.hasAcceptedSub;
        vm.title;
        vm.affiliationName;
        vm.line1;
        vm.line2;
        vm.city;
        vm.state;
        vm.country;
        vm.zipcode;
        vm.email;
        vm.phoneNumber;
        vm.fax;
        vm.day1;
        vm.day2;
        vm.day3;
        vm.companionFirstName;
        vm.companionLastName;
        vm.authorizations = {};
        vm.documentName;
        vm.documentFile;
        vm.authFirstName;
        vm.authLastName;
        vm.isRegistered;
        vm.registrationStatus;
        vm.currentid;

        vm.guestList = [];
        vm.sindex = 0;
        vm.smaxIndex = 0;
        vm.sfirstPage = true;
        vm.acceptanceStatusList = ['Accepted', 'Rejected'];

        //Functions
        vm.getGuestList = _getGuestList;
        vm.nextGuest = _nextGuest;
        vm.previousGuest = _previousGuest;
        vm.getFirstGuestPage = _getFirstGuestPage;
        vm.getLastGuestPage = _getLastGuestPage;
        vm.updateAcceptanceStatus = _updateAcceptanceStatus;
        vm.displayGuest = _displayGuest;
        vm.displayAuthorizations = _displayAuthorizations;
        vm.rejectRegisteredGuest = _rejectRegisteredGuest;
        vm.rejectedSelectedGuest = _rejectedSelectedGuest;
        vm.downloadPDFFile = _downloadPDFFile;
        vm.getDates = _getDates;
        vm.searchGuest = _searchGuest;

        //_getGuestList(vm.sindex);
        //_getDates();
        activate();

        //Functions:
        function activate() {
            _getGuestList(vm.sindex);
            _getDates();
        }

        function _getDates() {
            restApi.getDates().
                   success(function (data, status, headers, config) {
                       vm.datesList = data;
                       vm.date1 = vm.datesList[0];
                       vm.date2 = vm.datesList[1];
                       vm.date3 = vm.datesList[2];
                   }).
                   error(function (data, status, headers, config) {
                       vm.datesList = data;
                       vm.date1 = vm.datesList[0];
                       vm.date2 = vm.datesList[1];
                       vm.date3 = vm.datesList[2];
                       vm.toggleModal("error");
                   });
        }

        function _displayGuest(userID, firstName, lastName, acceptanceStatus, isRegistered, authorizationStatus, title, affiliationName,
            line1, line2, city, state, country, zipcode, email, phoneNumber, fax, day1, day2, day3, companionFirstName, companionLastName) {

            vm.modalFirstName = firstName;
            vm.modalLastName = lastName;
            vm.modalAcceptanceStatus = acceptanceStatus;
            vm.modalIsRegistered = isRegistered;
            vm.modalAuthorizationStatus = authorizationStatus;
            vm.modalTitle = title;
            vm.modalAffiliationName = affiliationName;
            vm.modalLine1 = line1;
            vm.modalLine2 = line2;
            vm.modalCity = city;
            vm.modalState = state;
            vm.modalCountry = country;
            vm.modalZipcode = zipcode;
            vm.modalEmail = email;
            vm.modalPhoneNumber = phoneNumber;
            vm.modalFax = fax;
            vm.modalDay1 = day1;
            vm.modalDay2 = day2;
            vm.modalDay3 = day3;
            vm.modalCompanionFirstName = companionFirstName;
            vm.modalCompanionLastName = companionLastName;
        }
        /*  Display dialogs */
        vm.obj = {};
        vm.toggleModal = function (action) {



            if (action == "errorfile") {

                vm.obj.title = "File Error",
                vm.obj.message1 = "Please refresh the page, a problem occured downloading a file.",

                vm.obj.message2 = vm.keyPop,
                vm.obj.label = "",
                vm.obj.okbutton = true,
                vm.obj.okbuttonText = "OK",
                vm.obj.cancelbutton = false,
                vm.obj.cancelbuttoText = "Cancel",
                vm.showConfirmModal = !vm.showConfirmModal;
                vm.okFunc = vm.deleteComplemetaryKey;
                vm.cancelFunc;

            }
            else if (action == "error") {
                vm.obj.title = "Server Error",
               vm.obj.message1 = "Please refresh the page and try again.",
               vm.obj.message2 = "",
               vm.obj.label = "",
               vm.obj.okbutton = true,
               vm.obj.okbuttonText = "OK",
               vm.obj.cancelbutton = false,
               vm.obj.cancelbuttoText = "Cancel",
               vm.showConfirmModal = !vm.showConfirmModal;
            }
        };

        function _displayAuthorizations(userID, firstName, lastName) {
            restApi.displayAuthorizations(userID).
                   success(function (data, status, headers, config) {
                       // this callback will be called asynchronously
                       // when the response is available
                       vm.authorizations = data;
                       vm.authFirstName = firstName;
                       vm.authLastName = lastName;
                   }).
                   error(function (data, status, headers, config) {
                       // called asynchronously if an error occurs
                       // or server returns response with an error status.
                       vm.authorizations = data;
                       vm.toggleModal("error");
                   });
        }
        //START PAGINATION CODE
        function _getGuestList(index) {
            vm.uploadingComp = true;
            restApi.getGuestList(index).
                   success(function (data, status, headers, config) {
                       vm.uploadingComp = false;
                       vm.smaxIndex = data.maxIndex;
                       if (vm.smaxIndex == 0) {
                           vm.sindex = 0;
                           vm.guestList = [];
                       }
                       else if (vm.sindex >= vm.smaxIndex) {
                           vm.sindex = vm.smaxIndex - 1;
                           _getGuestList(vm.sindex);
                       }
                       else {
                           vm.guestList = data.results;
                       }
                   }).
                   error(function (data, status, headers, config) {
                       vm.uploadingComp = false;
                       vm.toggleModal("error");
                   });
        }
            function _nextGuest() {
                if (vm.sindex < vm.smaxIndex - 1) {
                    vm.sindex += 1;
                    _getGuestList(vm.sindex);
                }
            }
        

        function _previousGuest() {
            if (vm.sindex > 0) {
                vm.sindex -= 1;
                _getGuestList(vm.sindex);
            }
        }

        function _getFirstGuestPage() {
            vm.sindex = 0;
            _getGuestList(vm.sindex);
        }

        function _getLastGuestPage() {
            vm.sindex = vm.smaxIndex - 1;
            _getGuestList(vm.sindex);
        }
        //----END PAGINATON CODE---

        function _updateAcceptanceStatus(userID, acceptanceStatus) {
            var localGuest = { ID: userID, status: acceptanceStatus };
            var element = document.getElementById("loading-" + userID);
            var btn = document.getElementById("button-" + userID);
            if (element != null && btn != null) {
                btn.style.display = "none";
                element.className = "glyphicon glyphicon-refresh glyphicon-refresh-animate";
            }
            restApi.updateAcceptanceStatus(localGuest)
                .success(function (data, status, headers, config) {                    
                    vm.guestList.forEach(function (guest, index) {
                        if (guest.userID == userID) {
                            guest.acceptanceStatus = acceptanceStatus;
                            if (element != null && btn != null) {
                                element.className = "";
                                btn.style.display = "";
                            }
                        }
                    })
                })

                .error(function (error) {
                    vm.toggleModal("error");
                });
        }

        function _rejectedSelectedGuest(userID) {
            vm.currentid = userID;
        }

        function _rejectRegisteredGuest() {
            if (vm.currentid != undefined) {
                restApi.rejectRegisteredGuest(vm.currentid)
                    .success(function (data, status, headers, config) {
                        vm.guestList.forEach(function (guest, index) {
                            if (guest.userID == vm.currentid) {
                                guest.isRegistered = false;
                                guest.registrationStatus = "Rejected";
                                guest.acceptanceStatus = "Rejected";
                                guest.day1 = false;
                                guest.day2 = false;
                                guest.day3 = false;
                            }
                        });


                    })


                .error(function (error) {
                    vm.toggleModal("error");
                });
            }
        }

        /* [Randy] download a file */
        function _downloadPDFFile(id) {
            restApi.getAuthorizationFile(id).
                success(function (data, status, headers, config) {
                    //window.open(data);

                    $("#file-" + id).attr("href", data.authorizationFile).attr("download", data.authorizationName);

                    //var file = new Blob([data]);
                    //saveAs(file);
                }).
                error(function (data, status, headers, config) {
                    vm.toggleModal("errorfile");
                });
        }

        /* [Randy] reset the link to default */
        function _resetDownloadLink(id) {
            $("#file-" + id).attr("href", "").removeAttr("download");
        }

        /* [Randy] Search within the list with a certain criteria */
        function _searchGuest() {
            vm.index = 0;
            var params = {index: vm.index, criteria: vm.criteria};
            restApi.searchGuest(params).
                success(function (data, status, headers, config) {
                    vm.smaxIndex = data.maxIndex;
                    if (vm.smaxIndex == 0) {
                        vm.sindex = 0;
                        vm.guestList = [];
                    }
                    else if (vm.sindex >= vm.smaxIndex) {
                        vm.sindex = vm.smaxIndex - 1;
                        _searchGuest(vm.sindex);
                    }
                    else {
                        vm.guestList = data.results;
                    }
                }).
                   error(function (data, status, headers, config) {
                       vm.toggleModal("error");
                   });
        }

    }
})();