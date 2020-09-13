select * from Students s inner join StudentClasses sc
on s.StudentID = sc.StudentID inner join Classes c on c.ClassID =sc.ClassID


alter table Students
add Grade varchar (200);

Update students
SET Grade = 10;
where StudentID = '4';

Update students 
SET Gender = 'Female'
where StudentID = '5';

Update students 
SET date_of_birth = '20 April 2007'
where StudentID = '6';

Update students 
SET date_of_birth = '20 April 2005'
where StudentID = '7';

Update students 
SET date_of_birth = '20 April 2001'
where StudentID = '4';