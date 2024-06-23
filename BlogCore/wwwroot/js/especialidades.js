var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblEspecialidades").DataTable({
        "ajax": {
            "url": "/Admin/Especialidades/GetAll", // Ruta para obtener los datos de las especialidades
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nombre", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Especialidades/Edit/${data}" class="btn btn-primary btn-sm"><i class="fa-solid fa-edit"></i> Editar</a>
                                <a onclick="eliminarEspecialidad(${data})" class="btn btn-danger btn-sm"><i class="fa-solid fa-trash-alt"></i> Eliminar</a>
                            </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.21/i18n/Spanish.json",
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

function eliminarEspecialidad(id) {
    swal({
        title: "¿Está seguro de eliminar?",
        text: "Este contenido no se puede recuperar.",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: `/Admin/Especialidades/Delete/${id}`, // Ruta para eliminar la especialidad
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
                    toastr.error("Error al intentar eliminar la especialidad.");
                }
            });
        }
    });
}
