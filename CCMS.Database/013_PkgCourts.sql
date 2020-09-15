-------------------------------------------------------------------------
-- PKG_COURTS: Perform CRUD operations on COURTS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_courts IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_courts (
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_court_details (
        pi_court_id IN courts.court_id%type,
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_exists_court (
        pi_court_name   IN courts.court_name%type,
        po_court_id     OUT courts.court_id%type,
        po_deleted      OUT courts.deleted%type
    );

    PROCEDURE p_create_new_court (
        pi_court_name   IN courts.court_name%type,
        po_court_id     OUT courts.court_id%type
    );

    PROCEDURE p_update_court (
        pi_court_id        IN courts.court_id%type,
        pi_court_name   IN courts.court_name%type
    );

    PROCEDURE p_delete_court (
        pi_court_id    IN courts.court_id%type
    );

END pkg_courts;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_courts IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_courts (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                court_id,
                court_name
            from
                courts
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_courts' || chr(10) || sqlerrm);
    END p_get_all_courts;
-------------------------------------------------------------------------
    PROCEDURE p_get_court_details (
        pi_court_id IN courts.court_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                court_id,
                court_name
            from
                courts
            where
                court_id = pi_court_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_court_details - pi_court_id: ' || pi_court_id
                || chr(10) || sqlerrm);
    END p_get_court_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_court (
        pi_court_name   IN courts.court_name%type,
        po_court_id     OUT courts.court_id%type,
        po_deleted      OUT courts.deleted%type
    ) IS
    BEGIN
        select
            court_id,
            deleted
        into
            po_court_id,
            po_deleted
        from
            courts
        where
            court_name = upper(pi_court_name);
    EXCEPTION
        when no_data_found then
            po_court_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_court - pi_court_name: ' || pi_court_name
                || chr(10) || sqlerrm);
    END p_exists_court;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_court (
        pi_court_name   IN courts.court_name%type,
        po_court_id     OUT courts.court_id%type
    ) IS
        v_deleted   courts.deleted%type;
    BEGIN
        p_exists_court(pi_court_name, po_court_id, v_deleted);
        if v_deleted is not null then
            update courts
            set deleted = null
            where court_id = po_court_id;
            
            return;
        end if;
        insert into courts (
            court_name
        ) values (
            upper(pi_court_name)
        );
        select
            court_id
            into
            po_court_id
        from
            courts
        where
            court_name = upper(pi_court_name);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_court - pi_court_name: ' || pi_court_name
                || chr(10) || sqlerrm);
    END p_create_new_court;
-------------------------------------------------------------------------
    PROCEDURE p_update_court (
        pi_court_id     IN courts.court_id%type,
        pi_court_name   IN courts.court_name%type
    ) IS
    BEGIN
        update courts
        set court_name = upper(pi_court_name)
        where court_id = pi_court_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_court - pi_court_id: ' || pi_court_id
                || '; pi_court_name: ' || pi_court_name
                || chr(10) || sqlerrm);
    END p_update_court;
-------------------------------------------------------------------------
    PROCEDURE p_delete_court (
        pi_court_id    IN courts.court_id%type
    ) IS
    BEGIN
        update courts
        set deleted = sysdate
        where court_id = pi_court_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_court - pi_court_id: ' || pi_court_id
                || chr(10) || sqlerrm);
    END p_delete_court;
-------------------------------------------------------------------------
END pkg_courts;
/
-------------------------------------------------------------------------