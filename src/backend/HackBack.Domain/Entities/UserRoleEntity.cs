namespace HackBack.Domain.Entities;

// ��� ������ ���� � ��������������, �� ��� ��� ����� ������ �������� ����������� (�����),
// ������ ����� � ����� ������ ����������, ������� ������ ��� ����� ������ ����� � ������ (���� ��� ��� � �� ��������).
public class UserRoleEntity
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}