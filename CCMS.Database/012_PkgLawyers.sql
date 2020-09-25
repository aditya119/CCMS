-------------------------------------------------------------------------
-- PKG_LAWYERS: Perform CRUD operations on LAWYERS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_lawyers IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_lawyers (
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_lawyer_details (
        pi_lawyer_id    IN lawyers.lawyer_id%type,
        po_cursor       OUT sys_refcursor
    );
    
    PROCEDURE p_exists_lawyer (
        pi_lawyer_email IN lawyers.lawyer_email%type,
        po_lawyer_id    OUT lawyers.lawyer_id%type,
        po_deleted      OUT lawyers.deleted%type
    );

    PROCEDURE p_create_new_lawyer (
        pi_lawyer_email     IN lawyers.lawyer_email%type,
        pi_lawyer_fullname  IN lawyers.lawyer_fullname%type,
        pi_lawyer_phone     IN lawyers.lawyer_phone%type,
        pi_lawyer_address   IN lawyers.lawyer_address%type,
        po_lawyer_id        OUT lawyers.lawyer_id%type
    );

    PROCEDURE p_update_lawyer (
        pi_lawyer_id        IN lawyers.lawyer_id%type,
        pi_lawyer_email     IN lawyers.lawyer_email%type,
        pi_lawyer_fullname  IN lawyers.lawyer_fullname%type,
        pi_lawyer_phone     IN lawyers.lawyer_phone%type,
        pi_lawyer_address   IN lawyers.lawyer_address%type
    );

    PROCEDURE p_delete_lawyer (
        pi_lawyer_id    IN lawyers.lawyer_id%type
    );

END pkg_lawyers;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_lawyers IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_lawyers (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                lawyer_id,
                lawyer_fullname || ' (' || lawyer_email || ')' lawyer_name_and_email
            from
                lawyers
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_lawyers' || chr(10) || sqlerrm);
    END p_get_all_lawyers;
-------------------------------------------------------------------------
    PROCEDURE p_get_lawyer_details (
        pi_lawyer_id    IN lawyers.lawyer_id%type,
        po_cursor       OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                lawyer_id,
                lawyer_fullname,
                lawyer_email,
                lawyer_phone,
                lawyer_address
            from
                lawyers
            where
                lawyer_id = pi_lawyer_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_lawyer_details - pi_lawyer_id: ' || pi_lawyer_id
                || chr(10) || sqlerrm);
    END p_get_lawyer_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_lawyer (
        pi_lawyer_email IN lawyers.lawyer_email%type,
        po_lawyer_id    OUT lawyers.lawyer_id%type,
        po_deleted      OUT lawyers.deleted%type
    ) IS
    BEGIN
        select
            lawyer_id,
            deleted
        into
            po_lawyer_id,
            po_deleted
        from
            lawyers
        where
            lawyer_email = lower(pi_lawyer_email);
    EXCEPTION
        when no_data_found then
            po_lawyer_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_lawyer - pi_lawyer_email: ' || pi_lawyer_email
                || chr(10) || sqlerrm);
    END p_exists_lawyer;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_lawyer (
        pi_lawyer_email     IN lawyers.lawyer_email%type,
        pi_lawyer_fullname  IN lawyers.lawyer_fullname%type,
        pi_lawyer_phone     IN lawyers.lawyer_phone%type,
        pi_lawyer_address   IN lawyers.lawyer_address%type,
        po_lawyer_id        OUT lawyers.lawyer_id%type
    ) IS
        v_deleted   lawyers.deleted%type;
    BEGIN
        p_exists_lawyer(pi_lawyer_email, po_lawyer_id, v_deleted);
        if v_deleted is not null then
            update lawyers
            set deleted = null
            where lawyer_id = po_lawyer_id;

            p_update_lawyer (
                po_lawyer_id,
                pi_lawyer_email,
                pi_lawyer_fullname,
                pi_lawyer_phone,
                pi_lawyer_address
            );
            return;
        end if;
        insert into lawyers (
            lawyer_email,
            lawyer_fullname,
            lawyer_phone,
            lawyer_address
        ) values (
            lower(pi_lawyer_email),
            pi_lawyer_fullname,
            pi_lawyer_phone,
            pi_lawyer_address
        );
        select
            lawyer_id
            into
            po_lawyer_id
        from
            lawyers
        where
            lawyer_email = lower(pi_lawyer_email);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_lawyer - pi_lawyer_email: ' || pi_lawyer_email
                || '; pi_lawyer_fullname: ' || pi_lawyer_fullname
                || '; pi_lawyer_phone: ' || pi_lawyer_phone
                || '; pi_lawyer_address: ' || pi_lawyer_address
                || chr(10) || sqlerrm);
    END p_create_new_lawyer;
-------------------------------------------------------------------------
    PROCEDURE p_update_lawyer (
        pi_lawyer_id        IN lawyers.lawyer_id%type,
        pi_lawyer_email     IN lawyers.lawyer_email%type,
        pi_lawyer_fullname  IN lawyers.lawyer_fullname%type,
        pi_lawyer_phone     IN lawyers.lawyer_phone%type,
        pi_lawyer_address   IN lawyers.lawyer_address%type
    ) IS
    BEGIN
        update lawyers
        set lawyer_email = lower(pi_lawyer_email),
            lawyer_fullname = pi_lawyer_fullname,
            lawyer_phone = pi_lawyer_phone,
            lawyer_address = pi_lawyer_address
        where lawyer_id = pi_lawyer_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_lawyer - pi_lawyer_id: ' || pi_lawyer_id
                || '; pi_lawyer_email: ' || pi_lawyer_email
                || '; pi_lawyer_fullname: ' || pi_lawyer_fullname
                || '; pi_lawyer_phone: ' || pi_lawyer_phone
                || '; pi_lawyer_address: ' || pi_lawyer_address
                || chr(10) || sqlerrm);
    END p_update_lawyer;
-------------------------------------------------------------------------
    PROCEDURE p_delete_lawyer (
        pi_lawyer_id    IN lawyers.lawyer_id%type
    ) IS
    BEGIN
        update lawyers
        set deleted = sysdate
        where lawyer_id = pi_lawyer_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_lawyer - pi_lawyer_id: ' || pi_lawyer_id
                || chr(10) || sqlerrm);
    END p_delete_lawyer;
-------------------------------------------------------------------------
END pkg_lawyers;
/
-------------------------------------------------------------------------