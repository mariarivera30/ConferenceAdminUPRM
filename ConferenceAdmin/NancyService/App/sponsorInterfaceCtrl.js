﻿//By: Heidi
(function () {
    'use strict';

    var controllerId = 'sponsorInterfaceCtrl';

    // TODO: replace app with your module name
    angular.module('app').controller(controllerId,
        ['$scope', '$http', 'restApi', sponsorInterfaceCtrl]);
    function sponsorInterfaceCtrl($scope, $http, restApi) {
        var vm = this;
        vm.activate = activate;
        vm.title = 'sponsorInterfaceCtrl';

        //For Admin Modal
        vm.temp;
        vm.selectedSponsorType;
        vm.amount;
        vm.benefits = {};
        vm.instructions;
        vm.loading = false;

        //For error modal:
        vm.obj = {
            title: "",
            message1: "",
            message2: "",
            label: "",
            okbutton: false,
            okbuttonText: "",
            cancelbutton: false,
            cancelbuttoText: "Cancel",
        };
        vm.okFunc;
        vm.cancelFunc;

        //Error Modal
        vm.toggleModal = function (action) {

            if (action == "error")
                vm.obj.title = "Server Error",
                vm.obj.message1 = "Please refresh the page and try again.",
                vm.obj.message2 = "",
                vm.obj.label = "",
                vm.obj.okbutton = true,
                vm.obj.okbuttonText = "OK",
                vm.obj.cancelbutton = false,
                vm.obj.cancelbuttoText = "Cancel",
                vm.showConfirmModal = !vm.showConfirmModal;
        };

        //Functions
        vm.getBenefits = _getBenefits;
        vm.saveBenefits = _saveBenefits;
        vm.getInstructions = _getInstructions;
        vm.saveInstructions = _saveInstructions;
        vm.selectedSponsor = _selectedSponsor;
        vm.clear = _clear;
        vm.reset = _reset;

        _getInstructions();

        function activate() {

        }

        //Reload original
        function _reset() {
            vm.instructions = vm.temp;
        }

        //Clear benefits modal
        function _clear() {
            vm.selectedSponsorType = "";
            vm.amount = "";
            vm.benefits = {};
        }

        //Sponsor type that has been selected for displaying benefits
        function _selectedSponsor(sponsorType) {
            vm.sponsorType = sponsorType;
            if (sponsorType == "Diamond") {
                _getBenefits("Diamond");
            }
            else if (sponsorType == "Platinum") {
                _getBenefits("Platinum");
            }

            else if (sponsorType == "Gold") {
                _getBenefits("Gold");
            }

            else if (sponsorType == "Silver") {
                _getBenefits("Silver");
            }

            else if (sponsorType == "Bronze") {
                _getBenefits("Bronze");
            }
        }

        //get a Sponsor type benefits
        function _getBenefits(sname) {
            restApi.getAdminSponsorBenefits(sname)
            .success(function (data, status, headers, config) {
                if (data != null && data != "") {
                    if (sname == "Diamond") {
                        vm.amount = data.diamondAmount;
                        vm.benefits = data.diamondBenefits;
                        $("#editSponsorBenefits").modal('show');
                    }
                    else if (sname == "Platinum") {
                        vm.amount = data.platinumAmount;
                        vm.benefits = data.platinumBenefits;
                        $("#editSponsorBenefits").modal('show');
                    }
                    else if (sname == "Gold") {
                        vm.amount = data.goldAmount;
                        vm.benefits = data.goldBenefits;
                        $("#editSponsorBenefits").modal('show');
                    }
                    else if (sname == "Silver") {
                        vm.amount = data.silverAmount;
                        vm.benefits = data.silverBenefits;
                        $("#editSponsorBenefits").modal('show');
                    }
                    else if (sname == "Bronze") {
                        vm.amount = data.bronzeAmount;
                        vm.benefits = data.bronzeBenefits;
                        $("#editSponsorBenefits").modal('show');
                    }
                }
            })

            .error(function (error) {
                vm.toggleModal('error');
            });
        }

        //update a sponsor type benefits
        function _saveBenefits() {
            vm.loading = true;
            var saveSponsor = {
                name: vm.sponsorType,
                amount: vm.amount,
                benefits: vm.benefits
            }
            restApi.saveAdminSponsorBenefits(saveSponsor)
            .success(function (data, status, headers, config) {
                if (data) {
                    vm.loading = false;
                    $("#editSponsorBenefits").modal('hide');
                    $("#updateConfirm").modal('show');
                }
            })
            .error(function (error) {
                vm.loading = false;
                $("#editSponsorBenefits").modal('hide');
                vm.toggleModal('error');
            });
        }

        //get Sponsor content information for editing
        function _getInstructions() {
            restApi.getInstructions()
            .success(function (data, status, headers, config) {
                if (data != null) {
                    vm.temp = data;
                    vm.instructions = data;
                    load();
                }
            })
            .error(function (error) {
                load();
                vm.toggleModal('error');
            });
        }

        //update Sponsor content
        function _saveInstructions() {
            vm.loading = true;
            var info = {
                instructions: vm.instructions
            }
            restApi.saveInstructions(info)
            .success(function (data, status, headers, config) {
                if (data) {
                    vm.temp = vm.instructions;
                    vm.loading = false;
                    $("#updateConfirm").modal('show');
                }
            })
            .error(function (error) {
                vm.loading = false;
                vm.toggleModal('error');
            });
        }

        //Avoid flashing when page loads
        var load = function () {
            if (document.getElementById('loading-icon') != null) {
                document.getElementById("loading-icon").style.visibility = "hidden";
            }
            document.getElementById("body").style.visibility = "visible";
        };
    }
})();