var dataTable;

$(document).ready(function () {
    cargarDatatable();
});


function cargarDatatable() {
    dataTable = $('#tblCitas').DataTable({
        "ajax": {
            "url": "/Admin/Citas/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "pacienteNombre", "width": "20%" },
            {
                "data": "fecha",
                "width": "15%",
                "render": function (data, type, row) {
                    // Formatear la fecha como "dd/MM/yyyy" 
                    var fecha = new Date(data); // Convertir la cadena de fecha a un objeto Date
                    var dia = fecha.getDate().toString().padStart(2, '0');
                    var mes = (fecha.getMonth() + 1).toString().padStart(2, '0'); // Los meses en JavaScript van de 0 a 11
                    var anio = fecha.getFullYear();
                    return dia + '/' + mes + '/' + anio;
                }
            },
            { "data": "hora", "width": "10%" },
            { "data": "medicoNombre", "width": "20%" },
            { "data": "observaciones", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                    <a href="/Admin/Citas/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                                        <i class="far fa-edit"></i> Editar
                                    </a>
                                    &nbsp;
                                    <a onclick="eliminarCita(${data})" class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                                        <i class="far fa-trash-alt"></i> Eliminar
                                    </a>
                                </div>`;
                },
                "width": "10%"
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "width": "100%"
    });
}

function eliminarCita(id) {
    swal({
        title: "¿Está seguro de querer borrar?",
        text: "¡Esta acción no se puede deshacer!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true
    }, function () {
        $.ajax({
            type: "DELETE",
            url: "/Admin/Citas/Delete/" + id,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                } else {
                    toastr.error(data.message);
                }
            },

            error: function (err) {
                console.error(err);
                toastr.error("Error al intentar eliminar la cita.");
            }
        });
    });
}
