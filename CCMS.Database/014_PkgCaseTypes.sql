-------------------------------------------------------------------------
-- PKG_CASE_TYPES: Perform CRUD operations on CASE_TYPES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_types IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_types (
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_case_type_details (
        pi_case_type_id IN case_types.case_type_id%type,
        po_cursor       OUT sys_refcursor
    );
    
    PROCEDURE p_exists_case_type (
        pi_case_type_name   IN case_types.case_type_name%type,
        po_case_type_id     OUT case_types.case_type_id%type,
        po_deleted          OUT case_types.deleted%type
    );

    PROCEDURE p_create_new_case_type (
        pi_case_type_name   IN case_types.case_type_name%type,
        po_case_type_id     OUT case_types.case_type_id%type
    );

    PROCEDURE p_update_case_type (
        pi_case_type_id     IN case_types.case_type_id%type,
        pi_case_type_name   IN case_types.case_type_name%type
    );

    PROCEDURE p_delete_case_type (
        pi_case_type_id    IN case_types.case_type_id%type
    );

END pkg_case_types;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_case_types IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_types (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_type_id,
                case_type_name
            from
                case_types
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_case_types' || chr(10) || sqlerrm);
    END p_get_all_case_types;
-------------------------------------------------------------------------
    PROCEDURE p_get_case_type_details (
        pi_case_type_id IN case_types.case_type_id%type,
        po_cursor       OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_type_id,
                case_type_name
            from
                case_types
            where
                case_type_id = pi_case_type_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_case_type_details - pi_case_type_id: ' || pi_case_type_id
                || chr(10) || sqlerrm);
    END p_get_case_type_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_case_type (
        pi_case_type_name   IN case_types.case_type_name%type,
        po_case_type_id     OUT case_types.case_type_id%type,
        po_deleted          OUT case_types.deleted%type
    ) IS
    BEGIN
        select
            case_type_id,
            deleted
        into
            po_case_type_id,
            po_deleted
        from
            case_types
        where
            case_type_name = upper(pi_case_type_name);
    EXCEPTION
        when no_data_found then
            po_case_type_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_case_type - pi_case_type_name: ' || pi_case_type_name
                || chr(10) || sqlerrm);
    END p_exists_case_type;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_case_type (
        pi_case_type_name   IN case_types.case_type_name%type,
        po_case_type_id     OUT case_types.case_type_id%type
    ) IS
        v_deleted   case_types.deleted%type;
    BEGIN
        p_exists_case_type(pi_case_type_name, po_case_type_id, v_deleted);
        if v_deleted is not null then
            update case_types
            set deleted = null
            where case_type_id = po_case_type_id;
            
            return;
        end if;
        insert into case_types (
            case_type_name
        ) values (
            upper(pi_case_type_name)
        );
        select
            case_type_id
            into
            po_case_type_id
        from
            case_types
        where
            case_type_name = upper(pi_case_type_name);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_case_type - pi_case_type_name: ' || pi_case_type_name
                || chr(10) || sqlerrm);
    END p_create_new_case_type;
-------------------------------------------------------------------------
    PROCEDURE p_update_case_type (
        pi_case_type_id     IN case_types.case_type_id%type,
        pi_case_type_name   IN case_types.case_type_name%type
    ) IS
    BEGIN
        update case_types
        set case_type_name = upper(pi_case_type_name)
        where case_type_id = pi_case_type_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case_type - pi_case_type_id: ' || pi_case_type_id
                || '; pi_case_type_name: ' || pi_case_type_name
                || chr(10) || sqlerrm);
    END p_update_case_type;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case_type (
        pi_case_type_id     IN case_types.case_type_id%type
    ) IS
    BEGIN
        update case_types
        set deleted = sysdate
        where case_type_id = pi_case_type_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_type - pi_case_type_id: ' || pi_case_type_id
                || chr(10) || sqlerrm);
    END p_delete_case_type;
-------------------------------------------------------------------------
END pkg_case_types;
/
-------------------------------------------------------------------------