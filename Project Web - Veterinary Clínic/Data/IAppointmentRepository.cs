using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Data
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAllWithUsersAsync();

        Task<IEnumerable<Appointment>> GetAppointmentsByVeterinarianAsync(string veterinarianId);

        Task<IEnumerable<Appointment>> GetAppointmentsForTomorrowAsync();

        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByVeterinarianAndDate(string veterinarianId, DateTime date);

        Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(string customerId);

        Task<IEnumerable<Appointment>> GetRecentAlertsForStaffAsync();

        IEnumerable<Appointment> GetAppointmentsByAnimalId(int animalId);

        


    }
}
