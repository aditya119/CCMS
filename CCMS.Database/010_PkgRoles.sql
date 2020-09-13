-------------------------------------------------------------------------
-- PKG_ROLES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_roles IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_roles (
        po_cursor   OUT sys_refcursor
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

END pkg_roles;
/
-------------------------------------------------------------------------