-------------------------------------------------------------------------
-- PKG_ROLES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_roles IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_roles (
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_role_id (
        pi_roles_csv IN VARCHAR2,
        po_role_id   OUT app_roles.role_id%type
    );

END pkg_roles;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_roles IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_roles (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                role_id,
                role_name,
                role_description
            from
                app_roles
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_roles' || chr(10) || sqlerrm);
    END p_get_all_roles;
-------------------------------------------------------------------------
    PROCEDURE p_get_role_id (
        pi_roles_csv IN VARCHAR2,
        po_role_id   OUT app_roles.role_id%type
    ) IS
    BEGIN
        select
            sum(role_id)
            into
            po_role_id
        from
            app_roles
        where
            role_name in (
                select
                    regexp_substr(pi_roles_csv,'[^,]+', 1, level) item
                from dual
                connect by
                    regexp_substr(pi_roles_csv, '[^,]+', 1, level) is not null);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_role_id - pi_roles_csv: ' || pi_roles_csv
                || chr(10) || sqlerrm);
    END p_get_role_id;
-------------------------------------------------------------------------
END pkg_roles;
/
-------------------------------------------------------------------------