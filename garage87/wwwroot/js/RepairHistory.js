//let Datatable;
//const baseUrl = window.location.origin;
//const apiUrl = `${baseUrl}/api/VehicleInterventions/1`;
//$(document).ready(function () {
//    Datatable = $('#tblData').DataTable({
//        processing: true,
//        serverSide: true,
//        paging: false,
//        info: false,
//        filter: true,
//        ajax: {
//            url: apiUrl,
//            type: 'GET',
//            datatype: 'json',
//            dataSrc: function (json) {
//                return json.length ? json : [];
//            }
//        },
//        columnDefs: [{
//            targets: ['_all'],
//            className: 'mdc-data-table__cell'
//        }],
//        columns: [
//            {
//                "data": "repairDate",
//                name: "repairDate",
//                render: function (data) {
//                    if (data) {
//                        const date = new Date(data);
//                        const options = { day: '2-digit', month: 'short', year: 'numeric' };
//                        return date.toLocaleDateString('en-GB', options).replace(/ /g, '-');
//                    }
//                    return '';
//                }
//            },
//            { "data": "vehicle.registration", name: "vehicle.registration" },
//            { "data": "employee.fullName", name: "employee.fullName" },
//            { "data": "total", name: "total" },
//            {
//                data: null,
//                render: renderButtons,
//                orderable: false,
//                searchable: false
//            }
//        ],
//        pageLength: 10,
//        order: [[0, 'asc']]
//    });

//    function renderButtons(data, type, row) {
//        return `
//            <button class="btn btn-primary btn-sm" onclick="Detail('${row.id}')" title="View Booking Details">
//                <i class="fa fa-eye"></i>
//            </button>
//        `;
//    }
//});
//function Detail(repairId) {
//    $.ajax({
//        url: '/Home/GetRepairDetails',
//        type: 'GET',
//        data: { repairId: repairId },
//        success: function (data) {
//            $('#vehicleRegistration').text(data.vehicleRegistration);
//            // Clear previous table content
//            $('#bookingDetailsBody').empty();

//            // Loop through repair details and append rows
//            if (data.details.length > 0) {
//                data.details.forEach(function (item) {
//                    $('#bookingDetailsBody').append(`
//                                                    <tr>
//                                                        <td>${item.name}</td>
//                                                        <td>${item.serviceCost}</td>
//                                                    </tr>
//                                                `);
//                });
//            } else {
//                $('#bookingDetailsBody').append('<tr><td colspan="2">No details available.</td></tr>');
//            }

//            // Show the modal
//            $('#bookingModal').modal('show');
//        },
//        error: function () {
//            swal("Error", "Failed to load repair details", "error");
//        }
//    });
//}

$('#vehiclebn').click(function () {
    $('#userIdModal').modal('show'); // Show the modal when the button is clicked
});
$(document).ready(function () {
    // Show the modal on page load
    $('#userIdModal').modal('show');

    // Handle the submission of the user ID
    $('#submitUserId').click(function () {
        const vehicleId = $('#userIdInput').val();
        if (vehicleId) {
            loadData(vehicleId); 
            $('#userIdModal').modal('hide'); 
        } else {
            alert("Please select a Vehicle.");
        }
    });
});
function loadData(vehicleId) {
    const baseUrl = window.location.origin; 
    const apiUrl = `${baseUrl}/api/VehicleInterventions/${vehicleId}`; 

    if ($.fn.DataTable.isDataTable('#tblData')) {
        // Clear existing data and load new data
        datatable.clear().destroy(); // Clear and destroy the existing DataTable
    }

    // Initialize the DataTable
     datatable = $('#tblData').DataTable({
        processing: true,
        serverSide: true,
        paging: false,
        info: false,
        filter: true,
        ajax: {
            url: apiUrl,
            type: 'GET',
            datatype: 'json',
            dataSrc: function (json) {
                return json.length ? json : [];
            }
        },
        columnDefs: [{
            targets: ['_all'],
            className: 'mdc-data-table__cell'
        }],
        columns: [
            {
                data: "repairDate",
                name: "repairDate",
                render: function (data) {
                    if (data) {
                        const date = new Date(data);
                        const options = { day: '2-digit', month: 'short', year: 'numeric' };
                        return date.toLocaleDateString('en-GB', options).replace(/ /g, '-');
                    }
                    return '';
                }
            },
            { data: "vehicle.registration", name: "vehicle.registration" },
            { data: "employee.fullName", name: "employee.fullName" },
            { data: "total", name: "total" },
            {
                data: null,
                render: renderButtons,
                orderable: false,
                searchable: false
            }
        ],
        pageLength: 10,
        order: [[0, 'asc']]
    });

    function renderButtons(data, type, row) {
        return `
            <button class="btn btn-primary btn-sm" onclick="Detail('${row.id}')" title="View Booking Details">
                <i class="fa fa-eye"></i>
            </button>
        `;
    }
}


function Detail(repairId) {
    $.ajax({
        url: '/Home/GetRepairDetails',
        type: 'GET',
        data: { repairId: repairId },
        success: function (data) {
            $('#vehicleRegistration').text(data.vehicleRegistration);
            // Clear previous table content
            $('#bookingDetailsBody').empty();

            // Loop through repair details and append rows
            if (data.details.length > 0) {
                data.details.forEach(function (item) {
                    $('#bookingDetailsBody').append(`
                                                    <tr>
                                                        <td>${item.name}</td>
                                                        <td>${item.serviceCost}</td>
                                                    </tr>
                                                `);
                });
            } else {
                $('#bookingDetailsBody').append('<tr><td colspan="2">No details available.</td></tr>');
            }

            // Show the modal
            $('#bookingModal').modal('show');
        },
        error: function () {
            swal("Error", "Failed to load repair details", "error");
        }
    });
}